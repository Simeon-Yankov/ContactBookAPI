using Bogus;
using ContactBookAPI.Domain.Exceptions;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using Xunit;
using static ContactBookAPI.Domain.Constants.DomainConstants.PhoneNumber;

namespace ContactBookAPI.Domain.UnitTests.ValueObjects;

public class PhoneNumberTests
{
    private readonly Faker _faker;

    public PhoneNumberTests()
    {
        _faker = new Faker();
    }

    public static IEnumerable<object[]> ValidPhoneNumbers()
    {
        yield return new object[] { "+" + new string('1', MinPhoneNumberLength - 1) }; // Minimum length
        yield return new object[] { "+" + new string('1', (MinPhoneNumberLength + MaxPhoneNumberLength) / 2 - 1) }; // Middle length
        yield return new object[] { "+" + new string('1', MaxPhoneNumberLength - 1) }; // Maximum length
    }

    public static IEnumerable<object[]> InvalidPhoneNumbers()
    {
        yield return new object[] { new string('1', MinPhoneNumberLength) }; // No plus
        yield return new object[] { "+" + new string('1', MinPhoneNumberLength - 2) }; // Too short
        yield return new object[] { "+" + new string('1', MaxPhoneNumberLength) }; // Too long
        yield return new object[] { "+123abc456" }; // Contains letters
        yield return new object[] { "+123 456" }; // Contains spaces
        yield return new object[] { "+" }; // Only plus
        yield return new object[] { "+123@#$456" }; // Special characters
        yield return new object[] { "123+456" }; // Plus in the middle
        yield return new object[] { "++123456" }; // Multiple plus signs
        yield return new object[] { "+12345a" }; // Ends with non-digit
    }

    [Theory]
    [MemberData(nameof(ValidPhoneNumbers))]
    public void Constructor_WithValidNumber_ShouldCreatePhoneNumber(string number)
    {
        // Act
        var phoneNumber = new PhoneNumber(number);

        // Assert
        phoneNumber.Number.Should().Be(number);
    }

    [Theory]
    [MemberData(nameof(InvalidPhoneNumbers))]
    public void Constructor_WithInvalidNumber_ShouldThrowInvalidPhoneNumberException(string number)
    {
        // Act
        var act = () => new PhoneNumber(number);

        // Assert
        act.Should().ThrowExactly<InvalidPhoneNumberException>();
    }

    [Fact]
    public void Constructor_WithRandomValidNumbers_ShouldCreatePhoneNumber()
    {
        // Arrange & Act & Assert
        for (int i = 0; i < 10; i++)
        {
            var length = _faker.Random.Int(MinPhoneNumberLength, MaxPhoneNumberLength);
            var number = "+" + string.Join("", _faker.Random.Digits(length - 1));

            var act = () => new PhoneNumber(number);
            act.Should().NotThrow();
        }
    }

    [Fact]
    public void Equals_WithSameNumber_ShouldReturnTrue()
    {
        // Arrange
        var digits = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var number = "+" + string.Join("", digits);
        var phoneNumber1 = new PhoneNumber(number);
        var phoneNumber2 = new PhoneNumber(number);

        // Act
        var result = phoneNumber1.Equals(phoneNumber2);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentNumbers_ShouldReturnFalse()
    {
        // Arrange
        var digits1 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var digits2 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var phoneNumber1 = new PhoneNumber("+" + string.Join("", digits1));
        var phoneNumber2 = new PhoneNumber("+" + string.Join("", digits2));

        // Act
        var result = phoneNumber1.Equals(phoneNumber2);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetHashCode_WithSameNumber_ShouldReturnSameHashCode()
    {
        // Arrange
        var digits = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var number = "+" + string.Join("", digits);
        var phoneNumber1 = new PhoneNumber(number);
        var phoneNumber2 = new PhoneNumber(number);

        // Act
        var hashCode1 = phoneNumber1.GetHashCode();
        var hashCode2 = phoneNumber2.GetHashCode();

        // Assert
        hashCode1.Should().Be(hashCode2);
    }

    [Fact]
    public void GetHashCode_WithDifferentNumbers_ShouldReturnDifferentHashCodes()
    {
        // Arrange
        var digits1 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var digits2 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var phoneNumber1 = new PhoneNumber("+" + string.Join("", digits1));
        var phoneNumber2 = new PhoneNumber("+" + string.Join("", digits2));

        // Act
        var hashCode1 = phoneNumber1.GetHashCode();
        var hashCode2 = phoneNumber2.GetHashCode();

        // Assert
        hashCode1.Should().NotBe(hashCode2);
    }

    [Fact]
    public void OperatorEquals_WithSameNumber_ShouldReturnTrue()
    {
        // Arrange
        var digits = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var number = "+" + string.Join("", digits);
        var phoneNumber1 = new PhoneNumber(number);
        var phoneNumber2 = new PhoneNumber(number);

        // Act & Assert
        (phoneNumber1 == phoneNumber2).Should().BeTrue();
    }

    [Fact]
    public void OperatorNotEquals_WithDifferentNumbers_ShouldReturnTrue()
    {
        // Arrange
        var digits1 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var digits2 = _faker.Random.Digits(MinPhoneNumberLength - 1);
        var phoneNumber1 = new PhoneNumber("+" + string.Join("", digits1));
        var phoneNumber2 = new PhoneNumber("+" + string.Join("", digits2));

        // Act & Assert
        (phoneNumber1 != phoneNumber2).Should().BeTrue();
    }
}
