using Bogus;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.Exceptions;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using static ContactBookAPI.Domain.Constants.DomainConstants.Address;

namespace ContactBookAPI.Domain.UnitTests.ValueObjects;

public class AddressTests
{
    private readonly Faker _faker;

    public AddressTests()
    {
        _faker = new Faker();
    }

    public static IEnumerable<object[]> ValidAddresses()
    {
        var defaultPhoneNumbers = new[] { new PhoneNumber("+1234567890") };

        yield return new object[]
        {
            new string('a', MinAddressLength),
            AddressType.Home,
            defaultPhoneNumbers
        }; // Minimum length

        yield return new object[]
        {
            new string('a', (MinAddressLength + MaxAddressLength) / 2),
            AddressType.Business,
            defaultPhoneNumbers
        }; // Middle length

        yield return new object[]
        {
            new string('a', MaxAddressLength),
            AddressType.Home,
            defaultPhoneNumbers
        }; // Maximum length
    }

    public static IEnumerable<object[]> InvalidAddresses()
    {
        var defaultPhoneNumbers = new[] { new PhoneNumber("+1234567890") };

        yield return new object[] { new string('a', MinAddressLength - 1), defaultPhoneNumbers }; // Too short
        yield return new object[] { new string('a', MaxAddressLength + 1), defaultPhoneNumbers }; // Too long
        yield return new object[] { string.Empty, defaultPhoneNumbers }; // Empty string
        yield return new object[] { null!, defaultPhoneNumbers }; // Null
    }

    [Theory]
    [MemberData(nameof(ValidAddresses))]
    public void Constructor_WithValidAddress_ShouldCreateAddress(
        string addressLine,
        AddressType addressType,
        IEnumerable<PhoneNumber> phoneNumbers)
    {
        // Act
        var address = new Address(addressLine, addressType, phoneNumbers);

        // Assert
        address.AddressLine.Should().Be(addressLine);
        address.AddressType.Should().Be(addressType);
        address.PhoneNumbers.Should().BeEquivalentTo(phoneNumbers);
    }

    [Theory]
    [MemberData(nameof(InvalidAddresses))]
    public void Constructor_WithInvalidAddress_ShouldThrowInvalidAddressException(
        string addressLine,
        IEnumerable<PhoneNumber> phoneNumbers)
    {
        // Act
        var act = () => new Address(addressLine, AddressType.Home, phoneNumbers);

        // Assert
        act.Should().ThrowExactly<IvnalidAddressException>();
    }

    [Fact]
    public void Constructor_WithMultiplePhoneNumbers_ShouldCreateAddressWithAllPhoneNumbers()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var phoneNumbers = new[]
        {
            new PhoneNumber("+1234567890"),
            new PhoneNumber("+0987654321"),
            new PhoneNumber("+1122334455")
        };

        // Act
        var address = new Address(addressLine, AddressType.Home, phoneNumbers);

        // Assert
        address.PhoneNumbers.Should().HaveCount(3);
        address.PhoneNumbers.Should().BeEquivalentTo(phoneNumbers);
    }

    [Fact]
    public void Constructor_WithEmptyPhoneNumbers_ShouldCreateAddressWithEmptyPhoneNumbers()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var phoneNumbers = Enumerable.Empty<PhoneNumber>();

        // Act
        var address = new Address(addressLine, AddressType.Home, phoneNumbers);

        // Assert
        address.PhoneNumbers.Should().BeEmpty();
    }

    [Fact]
    public void PhoneNumbers_ShouldReturnReadOnlyCollection()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var phoneNumbers = new[] { new PhoneNumber("+1234567890") };

        // Act
        var address = new Address(addressLine, AddressType.Home, phoneNumbers);

        // Assert
        address.PhoneNumbers.Should().BeAssignableTo<IReadOnlyCollection<PhoneNumber>>();
    }

    [Fact]
    public void Equals_WithSameValues_ShouldReturnTrue()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var phoneNumbers = new[] { new PhoneNumber("+1234567890") };
        var address1 = new Address(addressLine, AddressType.Home, phoneNumbers);
        var address2 = new Address(addressLine, AddressType.Home, phoneNumbers);

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldReturnFalse()
    {
        // Arrange
        var phoneNumbers = new[] { new PhoneNumber("+1234567890") };
        var address1 = new Address(new string('a', MinAddressLength), AddressType.Home, phoneNumbers);
        var address2 = new Address(new string('b', MinAddressLength), AddressType.Business, phoneNumbers);

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentPhoneNumbers_ShouldReturnFalse()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var address1 = new Address(addressLine, AddressType.Home, new[] { new PhoneNumber("+1234567890") });
        var address2 = new Address(addressLine, AddressType.Home, new[] { new PhoneNumber("+0987654321") });

        // Act
        var result = address1.Equals(address2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameValues_ShouldReturnSameHashCode()
    {
        // Arrange
        var addressLine = new string('a', MinAddressLength);
        var phoneNumbers = new[] { new PhoneNumber("+1234567890") };
        var address1 = new Address(addressLine, AddressType.Home, phoneNumbers);
        var address2 = new Address(addressLine, AddressType.Home, phoneNumbers);

        // Act
        var hashCode1 = address1.GetHashCode();
        var hashCode2 = address2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentValues_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var phoneNumbers = new[] { new PhoneNumber("+1234567890") };
        var address1 = new Address(new string('a', MinAddressLength), AddressType.Home, phoneNumbers);
        var address2 = new Address(new string('b', MinAddressLength), AddressType.Business, phoneNumbers);

        // Act
        var hashCode1 = address1.GetHashCode();
        var hashCode2 = address2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }
}
