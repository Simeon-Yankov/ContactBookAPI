﻿using ContactBookAPI.Domain.Exceptions;
using static ContactBookAPI.Domain.Constants.DomainConstants.Address;

namespace ContactBookAPI.Domain.ValueObjects;

public class Address : ValueObject
{
    private readonly List<PhoneNumber> _phoneNumbers;

    private Address(string addressLine, AddressType addressType)
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
        if (!(addressLine != null && MinAddressLength <= addressLine.Length && addressLine.Length <= MaxAddressLength))
            throw new IvnalidAddressException($"{nameof(Address)} must have between {MaxAddressLength} and {MaxAddressLength} symbols.");
    }

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
