using System.Text.RegularExpressions;
using ContactBookAPI.Domain.Exceptions;
using static ContactBookAPI.Domain.Constants.DomainConstants.PhoneNumber;

namespace ContactBookAPI.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public PhoneNumber(string number)
    {
        Validate(number);

        Number = number;
    }

    public string Number { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    private void Validate(string number)
    {
        if (MaxPhoneNumberLength <= number.Length && number.Length >= MaxPhoneNumberLength)
            throw new InvalidPhoneNumberException($"{nameof(PhoneNumber)} must have between {MaxPhoneNumberLength} and {MaxPhoneNumberLength} symbols.");

        if (!Regex.IsMatch(number, PhoneNumberRegularExpression))
            throw new InvalidPhoneNumberException($"{nameof(PhoneNumber)} must start with a '+' and contain only digits afterwards.");
    }
}
