namespace ContactBookAPI.Application.FunctionalTests.People.Commands;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.Common.Models;
using ContactBookAPI.Application.People.Commands.EditPerson;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;
using static Testing;

public class EditPersonTests : BaseTestFixture
{
    [Test]
    public async Task ShouldUpdatePersonFullName_WhenPersonExists()
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

        var command = new EditPersonCommand
        {
            Id = person.Id,
            FullName = "John Smith"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeTrue();

        var updatedPerson = await FindAsync<Person>(person.Id);
        updatedPerson.Should().NotBeNull();
        updatedPerson!.FullName.Should().Be("John Smith");
    }

    [Test]
    public async Task ShouldReturnFailure_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;
        var command = new EditPersonCommand
        {
            Id = nonExistentId,
            FullName = "New Name"
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeFalse();
    }

    [Test]
    public async Task ShouldReturnFailure_WhenNoChangesDetected()
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

        var command = new EditPersonCommand
        {
            Id = person.Id,
            FullName = "John Doe" // Same name as original
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
        var command = new EditPersonCommand
        {
            Id = 0,
            FullName = "John Smith"
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenIdIsNegative()
    {
        // Arrange
        var command = new EditPersonCommand
        {
            Id = -1,
            FullName = "John Smith"
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenFullNameIsEmpty()
    {
        // Arrange
        var command = new EditPersonCommand
        {
            Id = 1,
            FullName = ""
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var command = new EditPersonCommand
        {
            Id = 1,
            FullName = new string('A', 71) // Max length is 70
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldUpdatePersonFullName_CaseInsensitiveComparison()
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

        var command = new EditPersonCommand
        {
            Id = person.Id,
            FullName = "JOHN DOE" // Same name but different case
        };

        // Act
        var result = await SendAsync(command);

        // Assert
        result.Succeeded.Should().BeFalse(); // Should fail due to case-insensitive comparison
    }
}
