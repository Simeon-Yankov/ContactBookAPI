using ContactBookAPI.Domain.Enums;

namespace ContactBookAPI.Domain.ValueObjects;

public class Address : ValueObject
{
    private readonly List<PhoneNumber> _phoneNumbers;

    public Address(string addressLine, AddressType addressType)
    {
        Validate(addressLine);

        AddressLine = addressLine;
        AddressType = addressType;
        _phoneNumbers = new List<PhoneNumber>();
    }

    public Address(
        string addressLine,
        AddressType addressType,
        IEnumerable<PhoneNumber> phoneNumbers)
        : this(addressLine, addressType)
    {
        foreach (PhoneNumber phoneNumber in phoneNumbers)
        {
            _phoneNumbers.Add(phoneNumber);
        }
    }

    public string AddressLine { get; private set; }
    public AddressType AddressType { get; private set; }
    public IReadOnlyCollection<PhoneNumber> PhoneNumbers => _phoneNumbers.ToList().AsReadOnly();

    private void Validate(string addressLine)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ArgumentException("Address line cannot be empty.", nameof(addressLine));
    }

    //public override int GetHashCode()
    //{
    //    var hash = new HashCode();

    //    hash.Add(AddressLine);
    //    hash.Add(AddressType);

    //    foreach (var phoneNumber in _phoneNumbers.OrderBy(p => p.Number))
    //    {
    //        hash.Add(phoneNumber.Number);
    //    }

    //    return hash.ToHashCode();
    //}


    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return AddressLine;
        yield return AddressType;

        foreach (var phoneNumber in _phoneNumbers.OrderBy(p => p.Number))
        {
            yield return phoneNumber;
        }
    }
}
