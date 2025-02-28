using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;

namespace ContactBookAPI.Application.People.Queries.GetPerson;

public record GetPersonQuery : IRequest<PersonDto?>
{
    public int Id { get; init; }
}

public class GetPersonQueryValidator : AbstractValidator<GetPersonQuery>
{
    public GetPersonQueryValidator()
    {
        RuleFor(x => x.Id)
             .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class GetPersonQueryHandler : IRequestHandler<GetPersonQuery, PersonDto?>
{
    private readonly IApplicationDbContext _context;

    public GetPersonQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PersonDto?> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .Select(x => new PersonDto
            {
                Id = x.Id,
                FullName = x.FullName,
                Addresses = x.Addresses.Select(y => new AddressDto
                {
                    AddressLine = y.AddressLine,
                    AddressType = y.AddressType,
                    PhoneNumbers = y.PhoneNumbers.Select(x => x.Number).ToList(),
                }),
            })
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return person;
    }
}
