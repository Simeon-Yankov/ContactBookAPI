using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;

namespace ContactBookAPI.Application.People.Commands.CreatePerson;

public record CreatePersonCommand : IRequest<int>
{
    public string FullName { get; init; } = default!;
    public string HomeAddressLine { get; init; } = default!;
    public string BusinessAddressLine { get; init; } = default!;
    public IList<string> HomePhoneNumbers { get; init; } = [];
    public IList<string> BusinessPhoneNumbers { get; init; } = [];
}

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(70);

        RuleFor(x => x.HomeAddressLine)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.BusinessAddressLine)
            .NotEmpty()
            .MaximumLength(256);

        RuleFor(x => x.HomePhoneNumbers)
            .NotNull().WithMessage("Home Phone Numbers must not be null.")
             .ForEach(phone => phone
                    .NotEmpty().WithMessage("Each phone number must not be empty.")
                    .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
                );

        RuleFor(x => x.BusinessPhoneNumbers)
            .NotNull().WithMessage("Business Phone Numbers must not be null.")
             .ForEach(phone => phone
                    .NotEmpty().WithMessage("Each phone number must not be empty.")
                    .MaximumLength(15).WithMessage("Phone number cannot exceed 15 characters.")
                );
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
        var homePhoneNumbers = request.HomePhoneNumbers.Select(phone => new PhoneNumber(phone)).ToList();
        var businessPhoneNumbers = request.BusinessPhoneNumbers.Select(phone => new PhoneNumber(phone)).ToList();

        var homeAddress = new Address(request.HomeAddressLine, AddressType.Home, homePhoneNumbers);
        var businessAddress = new Address(request.BusinessAddressLine, AddressType.Business, businessPhoneNumbers);

        var person = new Person(
                request.FullName,
                homeAddress: homeAddress,
                businessAddress: businessAddress);

        await _context.People.AddAsync(person);
        await _context.SaveChangesAsync(cancellationToken);

        return person.Id;
    }
}
