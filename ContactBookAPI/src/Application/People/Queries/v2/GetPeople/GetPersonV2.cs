using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;

namespace ContactBookAPI.Application.People.Queries.v2.GetPeopleV2;

public record GetPersonV2Query : IRequest<PersonDto>
{
    public int Id { get; init; }
}

public class GetPersonV2QueryValidator : AbstractValidator<GetPersonV2Query>
{
    public GetPersonV2QueryValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class GetPersonV2QueryHandler : IRequestHandler<GetPersonV2Query, PersonDto>
{
    private readonly IApplicationDbContext _context;

    public GetPersonV2QueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonDto> Handle(GetPersonV2Query request, CancellationToken cancellationToken)
    {
        await Task.Run(() => { });

        throw new NotImplementedException();
    }
}
