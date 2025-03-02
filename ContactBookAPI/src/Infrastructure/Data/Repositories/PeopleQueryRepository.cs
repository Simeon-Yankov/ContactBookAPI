using System.Data;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;
using ContactBookAPI.Domain.Enums;
using Dapper;

namespace ContactBookAPI.Infrastructure.Data.Repositories;

public class PeopleQueryRepository : IPeopleQueryRepository
{
    private readonly IDbConnection _dbConnection;

    public PeopleQueryRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task<PersonDto?> GetPersonByIdAsync(int id, CancellationToken cancellationToken)
    {
        const string sql = """
                        SELECT *
            FROM 
                public."People" p
            LEFT JOIN 
                public."Address" a ON a."PersonId" = p."Id"
            LEFT JOIN 
                public."PhoneNumber" pn ON pn."AddressId" = a."Id"
            WHERE 
                p."Id" = @Id AND p."IsDeleted" = false
            """;

        var result = await _dbConnection.QueryAsync<dynamic>(
         new CommandDefinition(
             commandText: sql,
             parameters: new { Id = id },
             cancellationToken: cancellationToken
         ));

        var dic = MapToPersonDto(result);

        return dic.Values.FirstOrDefault();
    }

    public async Task<PaginatedList<PersonDto>> GetPeopleWithPaginationAsync(
        string? fullName,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        var whereClause = " WHERE NOT (p.\"IsDeleted\")";

        if (!string.IsNullOrWhiteSpace(fullName))
        {
            whereClause += " AND lower(p.\"FullName\") LIKE @FullNameLower";
            parameters.Add("FullNameLower", $"%{fullName.ToLower()}%");
        }

        var countSql = $"""
            SELECT count(*)::int
            FROM "People" AS p
            {whereClause}
        """;

        var totalCount = await _dbConnection.ExecuteScalarAsync<int>(
            new CommandDefinition(
                commandText: countSql,
                parameters: parameters,
                cancellationToken: cancellationToken
            ));

        var sql = $"""
                SELECT t."Id", t."FullName", t0."AddressLine", t0."AddressType", t0."PersonId", t0."Id", t0."Number", t0."AddressPersonId", t0."AddressId", t0."Id0"
                FROM (
                    SELECT p."Id", p."FullName"
                    FROM "People" AS p
                    {whereClause}
                    ORDER BY p."Id"
                    LIMIT @PageSize OFFSET @Offset
                ) AS t
                LEFT JOIN (
                    SELECT a."AddressLine", a."AddressType", a."PersonId", a."Id", p0."Number", p0."AddressPersonId", p0."AddressId", p0."Id" AS "Id0"
                    FROM "Address" AS a
                    LEFT JOIN "PhoneNumber" AS p0 ON a."PersonId" = p0."AddressPersonId" AND a."Id" = p0."AddressId"
                ) AS t0 ON t."Id" = t0."PersonId"
                ORDER BY t."Id", t0."PersonId", t0."Id", t0."AddressPersonId", t0."AddressId"
            """;

        parameters.Add("PageSize", pageSize);
        parameters.Add("Offset", (pageNumber - 1) * pageSize);

        var result = await _dbConnection.QueryAsync<dynamic>(
            new CommandDefinition(
                commandText: sql,
                parameters: parameters,
                cancellationToken: cancellationToken
            ));

        var dic = MapToPersonDto(result);
        var items = dic.Values.ToList();

        return new PaginatedList<PersonDto>(items, totalCount, pageNumber, pageSize);
    }

    #region Private Methods
    private static Dictionary<int, PersonDto> MapToPersonDto(IEnumerable<dynamic> result)
    {
        var personDictionary = new Dictionary<int, PersonDto>();
        var addressDictionary = new Dictionary<(int, int), AddressDto>();

        foreach (var row in result)
        {
            if (!personDictionary.TryGetValue((int)row.Id, out var person))
            {
                person = new PersonDto
                {
                    Id = (int)row.Id,
                    FullName = (string)row.FullName,
                    Addresses = new List<AddressDto>()
                };
                personDictionary.Add(person.Id, person);
            }

            if (row.AddressLine == null)
                continue;

            int addressTypeValue = (int)row.AddressType;
            var addressTypeEnum = (AddressType)addressTypeValue;

            var addressKey = (person.Id, (int)row.AddressType);
            if (!addressDictionary.TryGetValue(addressKey, out var address))
            {
                address = new AddressDto
                {
                    AddressLine = (string)row.AddressLine,
                    AddressType = addressTypeEnum.ToString(),
                    PhoneNumbers = new List<string>()
                };
                addressDictionary.Add(addressKey, address);
                ((List<AddressDto>)person.Addresses).Add(address);
            }

            if (row.Number != null)
            {
                ((List<string>)address.PhoneNumbers).Add((string)row.Number);
            }
        }

        return personDictionary ?? [];
    }

    #endregion
}
