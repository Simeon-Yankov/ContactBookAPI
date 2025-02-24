using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;

namespace ContactBookAPI.Domain.Entities;

public class Address
{
    private readonly HashSet<PhoneNumber> _phoneNumbers;

    public Address(string addressLine, AddressType addressType)
    {
        ValidateAddressLine(addressLine);

        AddressLine = addressLine;
        AddressType = addressType;
        _phoneNumbers = new HashSet<PhoneNumber>();
    }

    public string AddressLine { get; private set; }
    public AddressType AddressType { get; private set; }
    public IReadOnlyCollection<PhoneNumber> PhoneNumbers => _phoneNumbers.ToList().AsReadOnly();

    public void UpdateAddressLine(string addressLine)
    {
        ValidateAddressLine(addressLine);

        AddressLine = addressLine;
    }

    public bool AddPhoneNumber(PhoneNumber phoneNumber)
    {
        return _phoneNumbers.Add(phoneNumber);
    }

    public bool RemovePhoneNumber(PhoneNumber phoneNumber)
    {
        return _phoneNumbers.Remove(phoneNumber);
    }

    private void ValidateAddressLine(string addressLine)
    {
        if (string.IsNullOrWhiteSpace(addressLine))
            throw new ArgumentException("Address line cannot be empty.", nameof(addressLine));
    }
}
