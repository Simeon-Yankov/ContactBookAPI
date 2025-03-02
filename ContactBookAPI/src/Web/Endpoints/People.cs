using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Commands.CreatePerson;
using ContactBookAPI.Application.People.Commands.DeletePerson;
using ContactBookAPI.Application.People.Commands.EditPerson;
using ContactBookAPI.Application.People.Commands.UpdateAddress;
using ContactBookAPI.Application.People.Queries.v1.GetPeopleWithPagination;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;
using ContactBookAPI.Application.People.Queries.v2.GetPeopleWithPagination;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ContactBookAPI.Application.People.Queries.v2.GetPeopleV2;

namespace ContactBookAPI.Web.Endpoints;

public static class People
{
    public static void MapPeopleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/people")
            .WithTags("People")
            .WithOpenApi();

        group.MapGet("/", GetPeopleWithPagination);
        group.MapGet("/{id:int}", GetPerson);

        group.MapGet("/v2", GetPeopleWithPaginationV2);
        group.MapGet("/v2/{id:int}", GetPersonV2);

        group.MapPost("/", CreatePerson);
        group.MapPut("/", EditPerson);
        group.MapPut("/update-home-address", UpdateHomeAddress);
        group.MapPut("/update-business-address", UpdateBusinessAddress);
        group.MapDelete("/{id:int}", DeletePerson);
    }

    public static async Task<Ok<PaginatedList<PersonDto>>> GetPeopleWithPagination(
        ISender sender,
        [AsParameters] GetPeopleWithPaginationQuery query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }

    public static async Task<Results<Ok<PersonDto>, NotFound>> GetPerson(
        ISender sender,
        [AsParameters] GetPersonQuery query)
    {
        var result = await sender.Send(query);
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    /// <summary>
    /// Get Person with pagination using Dapper
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static async Task<Ok<PaginatedList<PersonDto>>> GetPeopleWithPaginationV2(
        ISender sender,
        [AsParameters] GetPeopleWithPaginationV2Query query)
    {
        var result = await sender.Send(query);
        return TypedResults.Ok(result);
    }


    /// <summary>
    /// Get Person using Dapper
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public static async Task<Results<Ok<PersonDto>, NotFound>> GetPersonV2(
        ISender sender,
        [AsParameters] GetPersonV2Query query)
    {
        var result = await sender.Send(query);
        return result is not null ? TypedResults.Ok(result) : TypedResults.NotFound();
    }

    public static async Task<Results<Created<int>, BadRequest<string>>> CreatePerson(
        ISender sender,
        CreatePersonCommand command)
    {
        var id = await sender.Send(command);
        return TypedResults.Created($"/api/person/{id}", id);
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> EditPerson(
        ISender sender,
        int id,
        EditPersonCommand command)
    {
        if (id != command.Id)
        {
            return TypedResults.BadRequest("Path ID does not match the request body ID.");
        }

        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.BadRequest(result.Message);
    }

    public record UpdateAddressRequest()
    {
        public int PersonId { get; init; }
        public string AddressLine { get; init; } = default!;
        public IList<string> PhoneNumbers { get; init; } = [];
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> UpdateHomeAddress(
        ISender sender,
        UpdateAddressRequest request)
    {
        var command = new UpdateAddressCommand
        {
            PersonId = request.PersonId,
            AddressLine = request.AddressLine,
            PhoneNumbers = request.PhoneNumbers,
            AddressType = Domain.Enums.AddressType.Home
        };

        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.BadRequest(result.Message);
    }

    public static async Task<Results<NoContent, NotFound, BadRequest<string>>> UpdateBusinessAddress(
            ISender sender,
            UpdateAddressRequest request)
    {
        var command = new UpdateAddressCommand
        {
            PersonId = request.PersonId,
            AddressLine = request.AddressLine,
            PhoneNumbers = request.PhoneNumbers,
            AddressType = Domain.Enums.AddressType.Business
        };

        var result = await sender.Send(command);

        if (result.Succeeded)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.BadRequest(result.Message);
    }

    /// <summary>
    /// Soft Delete
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static async Task<Results<NoContent, NotFound>> DeletePerson(
        ISender sender,
        int id)
    {
        var result = await sender.Send(new DeletePersonCommand { Id = id });
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
