namespace ContactBookAPI.Domain.Entities;

public class Person : BaseDeletableAuditableEntity
{
    private readonly HashSet<Address> _addresses;

    public Person(string fullName)
    {
        ValidateFullName(fullName);

        FullName = fullName;

        _addresses = new HashSet<Address>();
    }

    public string FullName { get; private set; }

    public IReadOnlyCollection<Address> Addresses => _addresses.ToList().AsReadOnly();

    public void UpdateFullName(string fullName)
    {
        ValidateFullName(fullName);

        FullName = fullName;
    }

    public bool AddAddress(Address address)
    {
        return _addresses.Add(address);
    }

    public bool RemoveAddress(Address address)
    {
        return _addresses.Remove(address);
    }

    private void ValidateFullName(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name cannot be empty.", nameof(fullName));
    }
}
