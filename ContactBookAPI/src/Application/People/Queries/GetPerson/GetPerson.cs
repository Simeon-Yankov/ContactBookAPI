using ContactBookAPI.Application.Common.Interfaces;

namespace ContactBookAPI.Application.People.Queries.GetPerson;

public record GetPersonQuery : IRequest<PersonDto>
{
}

public class GetPersonQueryValidator : AbstractValidator<GetPersonQuery>
{
    public GetPersonQueryValidator()
    {
    }
}

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDto>
{
    private readonly IApplicationDbContext _context;

    public GetPersonQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonDto> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        await Task.Run(() => { });

        throw new NotImplementedException();
    }
}
