using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;

namespace ContactBookAPI.Application.People.Queries.v2.GetPeopleWithPagination;

public record GetPeopleWithPaginationV2Query : IRequest<PaginatedList<PersonDto>>
{
    public string? FullName { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetPeopleWithPaginationV2QueryValidator : AbstractValidator<GetPeopleWithPaginationV2Query>
{
    public GetPeopleWithPaginationV2QueryValidator()
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

public class GetPeopleWithPaginationV2QueryHandler : IRequestHandler<GetPeopleWithPaginationV2Query, PaginatedList<PersonDto>>
{
    private readonly IPeopleQueryRepository _peopleQueryRepository;

    public GetPeopleWithPaginationV2QueryHandler(IPeopleQueryRepository peopleQueryRepository)
    {
        _peopleQueryRepository = peopleQueryRepository;
    }

    public async Task<PaginatedList<PersonDto>> Handle(GetPeopleWithPaginationV2Query request, CancellationToken cancellationToken)
    {
        var result = await _peopleQueryRepository.GetPeopleWithPaginationAsync(
            request.FullName,
            request.PageNumber,
            request.PageSize,
            cancellationToken);

        return result;
    }
}
