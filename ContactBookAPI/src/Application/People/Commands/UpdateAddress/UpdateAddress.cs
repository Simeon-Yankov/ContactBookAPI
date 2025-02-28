using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;

namespace ContactBookAPI.Application.People.Commands.UpdateAddress;

public record UpdateAddressCommand : IRequest<Result>
{
    public int PersonId { get; init; }
    public string AddressLine { get; init; } = default!;
    public AddressType AddressType { get; init; }
    public IList<string> PhoneNumbers { get; init; } = [];
}

public class UpdateAddressCommandValidator : AbstractValidator<UpdateAddressCommand>
{
    public UpdateAddressCommandValidator()
    {
        RuleFor(x => x.PersonId)
             .GreaterThan(0).WithMessage("Id must be greater than 0.");

        RuleFor(x => x.AddressLine)
            .NotEmpty().WithMessage("Address Line must not be empty.")
            .MaximumLength(256).WithMessage("Address Line cannot exceed 256 characters.");

        RuleFor(x => x.PhoneNumbers)
            .NotNull().WithMessage("Phone Numbers must not be null.")
             .ForEach(phone => phone
                    .NotEmpty().WithMessage("Each phone number must not be empty.")
                    .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
                );
    }
}

public class UpdateAddressCommandHandler : IRequestHandler<UpdateAddressCommand, Result>
{
    private readonly IApplicationDbContext _context;

    public UpdateAddressCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(UpdateAddressCommand request, CancellationToken cancellationToken)
    {
        var person = await _context.People
            .Include(x => x.Addresses)
            .Where(x => x.Id == request.PersonId)
            .FirstOrDefaultAsync(cancellationToken);

        if (person is null)
        {
            return Result.FailureWithMessages($"Person with ID {request.PersonId} was not found");
        }

        var phoneNumbers = request.PhoneNumbers.Select(x => new PhoneNumber(x)).ToList();
        var newAddress = new Address(request.AddressLine, request.AddressType, phoneNumbers);

        person.UpdateAddress(request.AddressType, newAddress);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
