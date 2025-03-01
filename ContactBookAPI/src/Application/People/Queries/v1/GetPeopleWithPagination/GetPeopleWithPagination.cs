using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Mappings;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;

namespace ContactBookAPI.Application.People.Queries.v1.GetPeopleWithPagination;

public record GetPeopleWithPaginationQuery : IRequest<PaginatedList<PersonDto>>
{
    public string? FullName { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetPeopleWithPaginationQueryValidator : AbstractValidator<GetPeopleWithPaginationQuery>
{
    public GetPeopleWithPaginationQueryValidator()
    {
        RuleFor(x => x.FullName)
           .MaximumLength(70)
           .When(x => !string.IsNullOrEmpty(x.FullName))
           .WithMessage("Full name must not exceed 70 characters");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
    }
}

public class GetPeopleWithPaginationQueryHandler : IRequestHandler<GetPeopleWithPaginationQuery, PaginatedList<PersonDto>>
{
    private readonly IApplicationDbContext _context;

    public GetPeopleWithPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedList<PersonDto>> Handle(GetPeopleWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var peopleQueriable = _context.People.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.FullName))
        {
            peopleQueriable = peopleQueriable.Where(x => x.FullName.ToLower().Contains(request.FullName.ToLower()));
        }

        var people = await peopleQueriable
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
            .OrderBy(x => x.Id)
            .PaginatedListAsync(request.PageNumber, request.PageSize);

        return people;
    }
}
