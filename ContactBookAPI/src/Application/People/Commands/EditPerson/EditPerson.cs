using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Domain.Entities;

namespace ContactBookAPI.Application.People.Commands.EditPerson;

public record EditPersonCommand : IRequest<Result>
{
    public int Id { get; init; } = default!;

    public string FullName { get; init; } = default!;
}

public class EditPersonCommandValidator : AbstractValidator<EditPersonCommand>
{
    public EditPersonCommandValidator()
    {
        RuleFor(x => x.Id)
             .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(70);
    }
}

public class EditPersonCommandHandler : IRequestHandler<EditPersonCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public EditPersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(EditPersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (person is null)
        {
            return Result.FailureWithMessages($"Person with ID {request.Id} was not found");
        }

        if (person!.FullName.Equals(request.FullName, StringComparison.OrdinalIgnoreCase))
        {
            return Result.FailureWithMessages($"No changes detected for Person with ID {request.Id}. The full name is already '{request.FullName}'.");
        }

        person.UpdateFullName(request.FullName);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
