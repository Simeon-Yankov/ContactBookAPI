using ContactBookAPI.Domain.Enums;

namespace ContactBookAPI.Application.People.Queries.v1.GetPerson;

public class PersonDto
{
    public int Id { get; init; }
    public string FullName { get; init; } = default!;
    public IEnumerable<AddressDto> Addresses { get; init; } = Array.Empty<AddressDto>();
}

public class AddressDto
{
    public string AddressLine { get; init; } = default!;
    public AddressType AddressType { get; init; }
    public IEnumerable<string> PhoneNumbers { get; init; } = Array.Empty<string>();
}
