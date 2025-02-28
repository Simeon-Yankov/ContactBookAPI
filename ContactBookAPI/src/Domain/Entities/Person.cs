using ContactBookAPI.Domain.Exceptions;

namespace ContactBookAPI.Domain.Entities;

public class Person : BaseDeletableAuditableEntity
{
    private readonly List<Address> _addresses;

    /// <summary>
    /// EF Core parameterless ctr
    /// </summary>
    private Person()
    {
        FullName = "";
        _addresses = [];
    }

    public Person(
        string fullName,
        Address homeAddress,
        Address businessAddress)
    {
        Validate(fullName, homeAddress, businessAddress);

        FullName = fullName;

        _addresses = new List<Address>();

        _addresses.Add(homeAddress);
        _addresses.Add(businessAddress);
    }

    public string FullName { get; private set; }

    public IReadOnlyCollection<Address> Addresses => _addresses.ToList().AsReadOnly();

    public void UpdateFullName(string fullName)
    {
        ValidateFullName(fullName);

        FullName = fullName;
    }

    public void UpdateAddress(AddressType type, Address newAddress)
    {
        if (newAddress is null)
            throw new InvalidPersonException($"{nameof(Address)} type not found. Type: {type}");

        var address = _addresses.FirstOrDefault(a => a.AddressType == type);

        if (address is null)
            throw new InvalidPersonException($"{nameof(Address)} type not found. Type: {type}");

        if (address.Equals(newAddress))
            throw new InvalidPersonException($"{nameof(Address)} already set.");

        if (!newAddress.AddressType.Equals(type))
            throw new InvalidPersonException($"{nameof(AddressType)} must match.");

        _addresses.Remove(address);
        _addresses.Add(newAddress);
    }

    #region Validations
    private void Validate(
        string fullName,
        Address homeAddress,
        Address businessAddress)
    {
        ValidateFullName(fullName);
        ValidateHomeAddress(homeAddress);
        ValidateBusinessAddress(businessAddress);
    }

    private void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new InvalidPersonException($"{nameof(FullName)} cannot be empty.");
    }

    private void ValidateHomeAddress(Address homeAddress)
    {
        if (homeAddress is null)
            throw new InvalidPersonException($"{nameof(Address)} cannot be null.");

        if (homeAddress.AddressType is not AddressType.Home)
            throw new InvalidPersonException($"Invalid {nameof(AddressType)}.");
    }

    private void ValidateBusinessAddress(Address businessAddress)
    {
        if (businessAddress is null)
            throw new InvalidPersonException($"{nameof(Address)} cannot be null.");

        if (businessAddress.AddressType is not AddressType.Business)
            throw new InvalidPersonException($"Invalid {nameof(AddressType)}.");
    }
    #endregion
}
