namespace ContactBookAPI.Application.FunctionalTests.People.Commands;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Commands.DeletePerson;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

using static Testing;

public class DeletePersonTests : BaseTestFixture
{
    [Test]
    public async Task ShouldDeletePerson_WhenPersonExists()
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

        var command = new DeletePersonCommand
        {
            Id = person.Id
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        // Verify person was deleted
        var deletedPerson = await FindAsync<Person>(person.Id);
        deletedPerson.Should().BeNull();
    }

    [Test]
    public async Task ShouldReturnFailure_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;
        var command = new DeletePersonCommand
        {
            Id = nonExistentId
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Test]
    public async Task ShouldFailValidation_WhenIdIsZero()
    {
        // Arrange
        var command = new DeletePersonCommand
        {
            Id = 0
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenIdIsNegative()
    {
        // Arrange
        var command = new DeletePersonCommand
        {
            Id = -1
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldDeletePersonAndRelatedData()
    {
        // Arrange - Create a person with addresses and phone numbers
        var homePhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber("+1234567890"),
            new PhoneNumber("+1122334455")
        };

        var businessPhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber("+0987654321"),
            new PhoneNumber("+9988776655")
        };

        var homeAddress = new Address(
            "123 Home St",
            AddressType.Home,
            homePhoneNumbers);

        var businessAddress = new Address(
            "456 Business Ave",
            AddressType.Business,
            businessPhoneNumbers);

        var person = new Person(
            "Jane Smith",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var command = new DeletePersonCommand
        {
            Id = person.Id
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        // Verify person was deleted
        var deletedPerson = await FindAsync<Person>(person.Id);
        deletedPerson.Should().BeNull();
    }
}
