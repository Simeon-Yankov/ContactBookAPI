namespace ContactBookAPI.Application.FunctionalTests.People.Queries.v1;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.People.Commands.EditPerson;
using ContactBookAPI.Application.People.Commands.UpdateAddress;
using ContactBookAPI.Application.People.Queries.v1.GetPerson;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;

using static Testing;

public class GetPersonTests : BaseTestFixture
{
    [Test]
    public async Task ShouldReturnPerson_WhenPersonExists()
    {
        // Arrange
        var homePhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber("+1234567890"),
            new PhoneNumber("+1122334455")
        };

        var businessPhoneNumbers = new List<PhoneNumber>
        {
            new PhoneNumber("+0987654321")
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
            "John Doe",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var query = new GetPersonQuery
        {
            Id = person.Id
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(person.Id);
        result.FullName.Should().Be("John Doe");

        result.Addresses.Should().HaveCount(2);

        var resultHomeAddress = result.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Home);
        resultHomeAddress.Should().NotBeNull();
        resultHomeAddress!.AddressLine.Should().Be("123 Home St");
        resultHomeAddress.PhoneNumbers.Should().HaveCount(2);
        resultHomeAddress.PhoneNumbers.Should().BeEquivalentTo(new[] { "+1234567890", "+1122334455" });

        var resultBusinessAddress = result.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Business);
        resultBusinessAddress.Should().NotBeNull();
        resultBusinessAddress!.AddressLine.Should().Be("456 Business Ave");
        resultBusinessAddress.PhoneNumbers.Should().HaveCount(1);
        resultBusinessAddress.PhoneNumbers.Should().BeEquivalentTo(new[] { "+0987654321" });
    }

    [Test]
    public async Task ShouldReturnNull_WhenPersonDoesNotExist()
    {
        // Arrange
        var nonExistentId = 999;
        var query = new GetPersonQuery
        {
            Id = nonExistentId
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().BeNull();
    }

    [Test]
    public async Task ShouldFailValidation_WhenIdIsZero()
    {
        // Arrange
        var query = new GetPersonQuery
        {
            Id = 0
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenIdIsNegative()
    {
        // Arrange
        var query = new GetPersonQuery
        {
            Id = -1
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldReturnCorrectAddressTypes()
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

        var query = new GetPersonQuery
        {
            Id = person.Id
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.Addresses.Should().Contain(a => a.AddressType == AddressType.Home);
        result.Addresses.Should().Contain(a => a.AddressType == AddressType.Business);
    }

    [Test]
    public async Task ShouldReturnEmptyPhoneNumbers_WhenAddressHasNoPhoneNumbers()
    {
        // Arrange
        var homeAddress = new Address(
            "123 Home St",
            AddressType.Home,
            new List<PhoneNumber>());

        var businessAddress = new Address(
            "456 Business Ave",
            AddressType.Business,
            new List<PhoneNumber> { new PhoneNumber("+0987654321") });

        var person = new Person(
            "John Doe",
            homeAddress,
            businessAddress);

        await AddAsync(person);

        var query = new GetPersonQuery
        {
            Id = person.Id
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        var resultHomeAddress = result!.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Home);
        resultHomeAddress.Should().NotBeNull();
        resultHomeAddress!.PhoneNumbers.Should().BeEmpty();
    }

    [Test]
    public async Task ShouldReturnCorrectData_AfterUpdatingPerson()
    {
        // Arrange - Create initial person
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

        // Get the person ID for later use
        int personId = person.Id;

        // Update the person using the EditPerson command
        var editCommand = new EditPersonCommand
        {
            Id = personId,
            FullName = "Jane Smith"
        };

        await SendAsync(editCommand);

        // Update the home address using the UpdateAddress command
        var updateAddressCommand = new UpdateAddressCommand
        {
            PersonId = personId,
            AddressLine = "789 New Home St",
            AddressType = AddressType.Home,
            PhoneNumbers = new List<string> { "+1122334455" }
        };

        await SendAsync(updateAddressCommand);

        // Now query for the updated person
        var query = new GetPersonQuery
        {
            Id = personId
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Jane Smith");

        var resultHomeAddress = result.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Home);
        resultHomeAddress.Should().NotBeNull();
        resultHomeAddress!.AddressLine.Should().Be("789 New Home St");
        resultHomeAddress.PhoneNumbers.Should().ContainSingle().Which.Should().Be("+1122334455");
    }
}
