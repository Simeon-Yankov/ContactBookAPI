
namespace ContactBookAPI.Domain.ValueObjects;

public class PhoneNumber : ValueObject
{
    public PhoneNumber(string number)
    {
        ValidatePhoneNumber(number);

        Number = number;
    }

    public string Number { get; private set; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Number;
    }

    private void ValidatePhoneNumber(string number)
    {
        if (string.IsNullOrEmpty(number))
            throw new ArgumentException("Phone number cannot be empty", nameof(number));
    }
}
