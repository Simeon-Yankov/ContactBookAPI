﻿using ContactBookAPI.Application.People.Queries.v1.GetPerson;

namespace ContactBookAPI.Application.People.Queries.v2.GetPeopleV2;

public record GetPersonV2Query : IRequest<PersonDto?>
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

public class GetPersonV2QueryHandler : IRequestHandler<GetPersonV2Query, PersonDto?>
{
    private readonly IPeopleQueryRepository _peopleQueryRepository;

    public GetPersonV2QueryHandler(IPeopleQueryRepository peopleQueryRepository)
    {
        _peopleQueryRepository = peopleQueryRepository;
    }

    public async Task<PersonDto?> Handle(GetPersonV2Query request, CancellationToken cancellationToken)
    {
        var result = await _peopleQueryRepository.GetPersonByIdAsync(request.Id, cancellationToken);

        return result;
    }
}
