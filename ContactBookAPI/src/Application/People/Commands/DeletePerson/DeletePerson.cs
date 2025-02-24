using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;

namespace ContactBookAPI.Application.People.Commands.DeletePerson;

public record DeletePersonCommand : IRequest<Result>
{
}

public class DeletePersonCommandValidator : AbstractValidator<DeletePersonCommand>
{
    public DeletePersonCommandValidator()
    {
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
        await Task.Run(() => { });

        throw new NotImplementedException();
    }
}
