using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;

namespace ContactBookAPI.Application.People.Commands.EditPerson;

public record EditPersonCommand : IRequest<Result>
{
}

public class EditPersonCommandValidator : AbstractValidator<EditPersonCommand>
{
    public EditPersonCommandValidator()
    {
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
        await Task.Run(() => { });
        throw new NotImplementedException();
    }
}
