using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.Exceptions;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace ContactBookAPI.Domain.UnitTests.Entities;

public class PersonTests
{
    private readonly Address _validHomeAddress;
    private readonly Address _validBusinessAddress;
    private readonly string _validFullName;

    public PersonTests()
    {
        var homePhoneNumbers = new[] { new PhoneNumber("+1234567890") };
        var businessPhoneNumbers = new[] { new PhoneNumber("+0987654321") };

        _validHomeAddress = new Address("Home Address", AddressType.Home, homePhoneNumbers);
        _validBusinessAddress = new Address("Business Address", AddressType.Business, businessPhoneNumbers);
        _validFullName = "John Doe";
    }

    [Fact]
    public void Constructor_WithValidParameters_ShouldCreatePerson()
    {
        // Act
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);

        // Assert
        person.FullName.Should().Be(_validFullName);
        person.Addresses.Should().HaveCount(2);
        person.Addresses.Should().Contain(_validHomeAddress);
        person.Addresses.Should().Contain(_validBusinessAddress);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Constructor_WithInvalidFullName_ShouldThrowInvalidPersonException(string? fullName)
    {
        // Act
        var act = () => new Person(fullName!, _validHomeAddress, _validBusinessAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void Constructor_WithNullHomeAddress_ShouldThrowInvalidPersonException()
    {
        // Act
        var act = () => new Person(_validFullName, null!, _validBusinessAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void Constructor_WithNullBusinessAddress_ShouldThrowInvalidPersonException()
    {
        // Act
        var act = () => new Person(_validFullName, _validHomeAddress, null!);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void Constructor_WithWrongHomeAddressType_ShouldThrowInvalidPersonException()
    {
        // Arrange
        var wrongHomeAddress = new Address("Wrong Type", AddressType.Business, new[] { new PhoneNumber("+1234567890") });

        // Act
        var act = () => new Person(_validFullName, wrongHomeAddress, _validBusinessAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void Constructor_WithWrongBusinessAddressType_ShouldThrowInvalidPersonException()
    {
        // Arrange
        var wrongBusinessAddress = new Address("Wrong Type", AddressType.Home, new[] { new PhoneNumber("+1234567890") });

        // Act
        var act = () => new Person(_validFullName, _validHomeAddress, wrongBusinessAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void UpdateFullName_WithValidName_ShouldUpdateName()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);
        var newName = "Jane Doe";

        // Act
        person.UpdateFullName(newName);

        // Assert
        person.FullName.Should().Be(newName);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void UpdateFullName_WithInvalidName_ShouldThrowInvalidPersonException(string? newName)
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);

        // Act
        var act = () => person.UpdateFullName(newName!);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void UpdateAddress_WithValidHomeAddress_ShouldUpdateHomeAddress()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);
        var newHomeAddress = new Address("New Home", AddressType.Home, new[] { new PhoneNumber("+1111111111") });

        // Act
        person.UpdateAddress(AddressType.Home, newHomeAddress);

        // Assert
        person.Addresses.Should().Contain(newHomeAddress);
        person.Addresses.Should().NotContain(_validHomeAddress);
        person.Addresses.Should().Contain(_validBusinessAddress);
    }

    [Fact]
    public void UpdateAddress_WithValidBusinessAddress_ShouldUpdateBusinessAddress()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);
        var newBusinessAddress = new Address("New Business", AddressType.Business, new[] { new PhoneNumber("+2222222222") });

        // Act
        person.UpdateAddress(AddressType.Business, newBusinessAddress);

        // Assert
        person.Addresses.Should().Contain(newBusinessAddress);
        person.Addresses.Should().NotContain(_validBusinessAddress);
        person.Addresses.Should().Contain(_validHomeAddress);
    }

    [Fact]
    public void UpdateAddress_WithNullAddress_ShouldThrowArgumentNullException()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);

        // Act
        var act = () => person.UpdateAddress(AddressType.Home, null!);

        // Assert
        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void UpdateAddress_WithSameAddress_ShouldThrowInvalidPersonException()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);
        var sameHomeAddress = new Address(_validHomeAddress.AddressLine, AddressType.Home, _validHomeAddress.PhoneNumbers);

        // Act
        var act = () => person.UpdateAddress(AddressType.Home, sameHomeAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void UpdateAddress_WithWrongAddressType_ShouldThrowInvalidPersonException()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);
        var wrongTypeAddress = new Address("Wrong Type", AddressType.Home, new[] { new PhoneNumber("+1234567890") });

        // Act
        var act = () => person.UpdateAddress(AddressType.Business, wrongTypeAddress);

        // Assert
        act.Should().ThrowExactly<InvalidPersonException>();
    }

    [Fact]
    public void Addresses_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var person = new Person(_validFullName, _validHomeAddress, _validBusinessAddress);

        // Act & Assert
        person.Addresses.Should().BeAssignableTo<IReadOnlyCollection<Address>>();
    }
}
