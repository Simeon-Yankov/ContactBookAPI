using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;

namespace ContactBookAPI.Application.People;

public interface IPeopleQueryRepository
{
    Task<PersonDto?> GetPersonByIdAsync(int id, CancellationToken cancellationToken);
    Task<PaginatedList<PersonDto>> GetPeopleWithPaginationAsync(string? fullName, int pageNumber, int pageSize, CancellationToken cancellationToken);
}
