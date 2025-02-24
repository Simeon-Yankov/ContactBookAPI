using ContactBookAPI.Application.Common.Interfaces;

namespace ContactBookAPI.Application.People.Commands.CreatePerson;

public record CreatePersonCommand : IRequest<int>
{
    public string FullName { get; init; } = default!;
}

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
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
        await Task.Run(() => { });

        throw new NotImplementedException();
    }
}
