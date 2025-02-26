using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;

namespace ContactBookAPI.Application.People.Commands.DeletePerson;

public record DeletePersonCommand : IRequest<Result>
{
    public int Id { get; init; } = default!;
}

public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {
        RuleFor(x => x.Id)
             .GreaterThan(0).WithMessage("Id must be greater than 0.");
    }
}

public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public DeletePersonCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .Where(x => x.Id == request.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (person is null) 
        {
            return Result.FailureWithMessages($"Person with ID {request.Id} was not found");
        }

        _context.People.Remove(person);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
