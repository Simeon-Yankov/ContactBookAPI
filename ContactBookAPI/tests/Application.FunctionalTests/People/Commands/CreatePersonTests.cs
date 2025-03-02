namespace ContactBookAPI.Application.FunctionalTests.People.Commands;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.People.Commands.CreatePerson;
using ContactBookAPI.Domain.Constants;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;

using static Testing;

public class CreatePersonTests : BaseTestFixture
{
    [Test]
    public async Task ShouldCreatePerson()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "+1234567890" },
            BusinessPhoneNumbers = new List<string> { "+0987654321" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        var person = await FindAsync<Person>(result.Data);

        person.Should().NotBeNull();
        person!.FullName.Should().Be(command.FullName);
        person.Addresses.Should().HaveCount(2);

        var homeAddress = person.Addresses.First(a => a.AddressType == AddressType.Home);
        homeAddress.AddressLine.Should().Be(command.HomeAddressLine);
        homeAddress.PhoneNumbers.Should().HaveCount(1);
        homeAddress.PhoneNumbers.First().Number.Should().Be(command.HomePhoneNumbers[0]);

        var businessAddress = person.Addresses.First(a => a.AddressType == AddressType.Business);
        businessAddress.AddressLine.Should().Be(command.BusinessAddressLine);
        businessAddress.PhoneNumbers.Should().HaveCount(1);
        businessAddress.PhoneNumbers.First().Number.Should().Be(command.BusinessPhoneNumbers[0]);
    }

    [Test]
    public async Task ShouldCreatePersonWithMultiplePhoneNumbers()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "+1234567890", "+1122334455" },
            BusinessPhoneNumbers = new List<string> { "+0987654321", "+9988776655" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        var person = await FindAsync<Person>(result.Data);

        person.Should().NotBeNull();
        var homeAddress = person!.Addresses.First(a => a.AddressType == AddressType.Home);
        homeAddress.PhoneNumbers.Should().HaveCount(2);
        homeAddress.PhoneNumbers.Select(p => p.Number).Should().BeEquivalentTo(command.HomePhoneNumbers);

        var businessAddress = person.Addresses.First(a => a.AddressType == AddressType.Business);
        businessAddress.PhoneNumbers.Should().HaveCount(2);
        businessAddress.PhoneNumbers.Select(p => p.Number).Should().BeEquivalentTo(command.BusinessPhoneNumbers);
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task ShouldRequireFullName(string? fullName)
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = fullName!,
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "+1234567890" },
            BusinessPhoneNumbers = new List<string> { "+0987654321" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task ShouldRequireHomeAddress(string? homeAddress)
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = homeAddress!,
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "+1234567890" },
            BusinessPhoneNumbers = new List<string> { "+0987654321" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase(null)]
    public async Task ShouldRequireBusinessAddress(string? businessAddress)
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = businessAddress!,
            HomePhoneNumbers = new List<string> { "+1234567890" },
            BusinessPhoneNumbers = new List<string> { "+0987654321" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequirePhoneNumbers()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = null!,
            BusinessPhoneNumbers = null!
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireNonEmptyPhoneNumbers()
    {
        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "" },
            BusinessPhoneNumbers = new List<string> { "" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireValidPhoneNumberLength()
    {
        var invalidMaxLengthPhoneNumber = new string('1', DomainConstants.PhoneNumber.MaxPhoneNumberLength);

        // Arrange
        var command = new CreatePersonCommand
        {
            FullName = "John Doe",
            HomeAddressLine = "123 Home St",
            BusinessAddressLine = "456 Business Ave",
            HomePhoneNumbers = new List<string> { "+" + invalidMaxLengthPhoneNumber }, // 16 characters
            BusinessPhoneNumbers = new List<string> { "+" + invalidMaxLengthPhoneNumber }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }
}
