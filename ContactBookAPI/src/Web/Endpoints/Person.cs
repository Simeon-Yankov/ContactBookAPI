
using ContactBookAPI.Application.People.Commands.CreatePerson;
using ContactBookAPI.Application.People.Queries.GetPerson;
using MediatR;

namespace ContactBookAPI.Web.Endpoints;

public class Person : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app
            .MapGroup(this)
            .MapGet(GetPerson)
            .MapPost(CreateTodoItem)
            .MapGet(Test);
    }

    public async Task<PersonDto?> GetPerson(
        ISender sender,
        [AsParameters] GetPersonQuery query)
    {
        var result = await sender.Send(query);

        return result;
    }

    public async Task<int> CreateTodoItem(ISender sender, CreatePersonCommand command)
    {
        return await sender.Send(command);
    }

    public bool Test()
    {
        return true;
    }
}
