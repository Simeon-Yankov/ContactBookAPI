using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Domain.Entities;

namespace ContactBookAPI.Application.People.Commands.CreatePerson;

public record CreatePersonCommand : IRequest<int>
{
    public string FullName { get; init; } = default!;
}

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(70);
    }
}

public class CreatePersonCommandHandler : IRequestHandler<CreatePersonCommand, int>
{
    private readonly IApplicationDbContext _context;

    public CreatePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        var person = new Person(request.FullName);

        _context.People.Add(person);

        await _context.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
