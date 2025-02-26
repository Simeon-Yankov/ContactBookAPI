using ContactBookAPI.Application.People.Commands.CreatePerson;
using ContactBookAPI.Application.People.Queries.GetPerson;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ContactBookAPI.Web.Endpoints;

public static class People
{
    public static void MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/person")
            .WithTags("Person")
            .WithOpenApi();

        group.MapGet("/", GetPerson);
        group.MapPost("/", CreatePerson);
        group.MapGet("/test", Test);
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
        try
        {
            var id = await sender.Send(command);
            return TypedResults.Created($"/api/person/{id}", id);
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
    }

    public static Ok<bool> Test()
    {
        return TypedResults.Ok(true);
    }
}
