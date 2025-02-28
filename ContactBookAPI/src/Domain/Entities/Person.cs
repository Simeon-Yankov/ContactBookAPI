using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;

namespace ContactBookAPI.Domain.Entities;

public class Person : BaseDeletableAuditableEntity
{
    private readonly HashSet<Address> _addresses;

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

        _addresses = new HashSet<Address>();

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
        ArgumentNullException.ThrowIfNull(newAddress);

        var address = _addresses.FirstOrDefault(a => a.AddressType == type);

        if (address is null)
        {
            throw new Exception($"Address type not found. Type: {type}");
        }

        // TODO: FIx
        //if (address.Equals(newAddress))
        //{
        //    throw new Exception("The address is Same.");
        //}

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
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
    }

    private void ValidateHomeAddress(Address homeAddress)
    {
        ArgumentNullException.ThrowIfNull(homeAddress);

        if (homeAddress.AddressType is not AddressType.Home)
            throw new ArgumentException("Invalid AddressType.");
    }

    private void ValidateBusinessAddress(Address businessAddress)
    {
        ArgumentNullException.ThrowIfNull(businessAddress);

        if (businessAddress.AddressType is not AddressType.Business)
            throw new ArgumentException("Invalid AddressType.");
    }
    #endregion
}
