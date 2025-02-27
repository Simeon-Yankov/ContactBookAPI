using ContactBookAPI.Application.People.Commands.CreatePerson;
using ContactBookAPI.Application.People.Commands.DeletePerson;
using ContactBookAPI.Application.People.Commands.EditPerson;
using ContactBookAPI.Application.People.Queries.GetPerson;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ContactBookAPI.Web.Endpoints;

public static class People
{
    public static void MapPeopleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/people")
            .WithTags("People")
            .WithOpenApi();

        group.MapGet("/", GetPerson);
        group.MapPost("/", CreatePerson);
        group.MapPut("/", EditPerson);
        group.MapDelete("/{id:int}", DeletePerson);
    }

    public static async Task<Results<Ok<PersonDto>, NotFound>> GetPerson(
        ISender sender,
        [AsParameters] GetPersonQuery query)
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
