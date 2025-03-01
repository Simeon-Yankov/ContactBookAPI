namespace ContactBookAPI.Application.FunctionalTests.People.Commands;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Commands.UpdateAddress;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

using static Testing;

public class UpdateAddressTests : BaseTestFixture
{
    [Test]
    public async Task ShouldUpdateHomeAddress_WhenPersonExists()
    {
        // Arrange
        var homePhoneNumber = new PhoneNumber("+1234567890");
        var businessPhoneNumber = new PhoneNumber("+0987654321");

        var homeAddress = new Address(
            "123 Home St",
            AddressType.Home,
            new List<PhoneNumber> { homePhoneNumber });

        var businessAddress = new Address(
            "456 Business Ave",
            AddressType.Business,
            new List<PhoneNumber> { businessPhoneNumber });

        var person = new Person(
            "John Doe",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var command = new UpdateAddressCommand
        {
            PersonId = person.Id,
            AddressLine = "789 New Home St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1122334455" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        var updatedPerson = await FindAsync<Person>(person.Id);
        updatedPerson.Should().NotBeNull();

        var updatedHomeAddress = updatedPerson!.Addresses
            .FirstOrDefault(a => a.AddressType == AddressType.Home);

        updatedHomeAddress.Should().NotBeNull();
        updatedHomeAddress!.AddressLine.Should().Be("789 New Home St");
        updatedHomeAddress.PhoneNumbers.Should().HaveCount(1);
        updatedHomeAddress.PhoneNumbers.First().Number.Should().Be("+1122334455");
    }

    [Test]
    public async Task ShouldUpdateBusinessAddress_WhenPersonExists()
    {
        // Arrange
        var homePhoneNumber = new PhoneNumber("+1234567890");
        var businessPhoneNumber = new PhoneNumber("+0987654321");

        var homeAddress = new Address(
            "123 Home St",
            AddressType.Home,
            new List<PhoneNumber> { homePhoneNumber });

        var businessAddress = new Address(
            "456 Business Ave",
            AddressType.Business,
            new List<PhoneNumber> { businessPhoneNumber });

        var person = new Person(
            "John Doe",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var command = new UpdateAddressCommand
        {
            PersonId = person.Id,
            AddressLine = "999 New Business Ave",
            AddressType = AddressType.Business,
            PhoneNumbers = new List<string> { "+9988776655", "+5544332211" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        var updatedPerson = await FindAsync<Person>(person.Id);
        updatedPerson.Should().NotBeNull();

        var updatedBusinessAddress = updatedPerson!.Addresses
            .FirstOrDefault(a => a.AddressType == AddressType.Business);

        updatedBusinessAddress.Should().NotBeNull();
        updatedBusinessAddress!.AddressLine.Should().Be("999 New Business Ave");
        updatedBusinessAddress.PhoneNumbers.Should().HaveCount(2);
        updatedBusinessAddress.PhoneNumbers.Select(p => p.Number)
            .Should().BeEquivalentTo(new[] { "+9988776655", "+5544332211" });
    }

    [Test]
    public async Task ShouldReturnFailure_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;
        var command = new UpdateAddressCommand
        {
            PersonId = nonExistentId,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1234567890" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPersonIdIsZero()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 0,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1234567890" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPersonIdIsNegative()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = -1,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1234567890" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenAddressLineIsEmpty()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 1,
            AddressLine = "",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1234567890" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenAddressLineExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 1,
            AddressLine = new string('A', 257), // Max length is 256
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1234567890" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPhoneNumbersIsNull()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 1,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = null!
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPhoneNumberIsEmpty()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 1,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "" }
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPhoneNumberExceedsMaxLength()
    {
        // Arrange
        var command = new UpdateAddressCommand
        {
            PersonId = 1,
            AddressLine = "123 New St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { new string('1', 16) } // Max length is 15
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldUpdateAddressWithMultiplePhoneNumbers()
    {
        // Arrange
        var homePhoneNumber = new PhoneNumber("+1234567890");
        var businessPhoneNumber = new PhoneNumber("+0987654321");

        var homeAddress = new Address(
            "123 Home St",
            AddressType.Home,
            new List<PhoneNumber> { homePhoneNumber });

        var businessAddress = new Address(
            "456 Business Ave",
            AddressType.Business,
            new List<PhoneNumber> { businessPhoneNumber });

        var person = new Person(
            "John Doe",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var command = new UpdateAddressCommand
        {
            PersonId = person.Id,
            AddressLine = "789 New Home St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1122334455", "+6677889900", "+1231231234" }
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        var updatedPerson = await FindAsync<Person>(person.Id);
        updatedPerson.Should().NotBeNull();

        var updatedHomeAddress = updatedPerson!.Addresses
            .FirstOrDefault(a => a.AddressType == AddressType.Home);

        updatedHomeAddress.Should().NotBeNull();
        updatedHomeAddress!.PhoneNumbers.Should().HaveCount(3);
        updatedHomeAddress.PhoneNumbers.Select(p => p.Number)
            .Should().BeEquivalentTo(new[] { "+1122334455", "+6677889900", "+1231231234" });
    }
}
