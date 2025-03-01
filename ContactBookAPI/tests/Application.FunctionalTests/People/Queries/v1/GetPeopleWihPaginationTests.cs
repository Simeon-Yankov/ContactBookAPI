namespace ContactBookAPI.Application.FunctionalTests.People.Queries.v1;

using ContactBookAPI.Application.Common.Exceptions;
using ContactBookAPI.Application.People.Queries.v1.GetPeopleWithPagination;
using ContactBookAPI.Domain.Entities;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static Testing;

[TestFixture]
public class GetPeopleWithPaginationTests : BaseTestFixture
{
    [OneTimeSetUp]
    public async Task TestFixtureSetUp()
    {
        await ResetState();
    }

    [SetUp]
    public async Task SetUp()
    {
        await ResetState();
    }

    [TearDown]
    public async Task TearDown()
    {
        await ResetState();
    }

    [Test]
    public async Task ShouldReturnAllPeople_WhenNoFilterIsProvided()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test Person 1"));
        await AddAsync(CreatePerson("Test Person 2"));
        await AddAsync(CreatePerson("Test Person 3"));

        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().HaveCount(3);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(1);
        result.HasNextPage.Should().BeFalse();
        result.HasPreviousPage.Should().BeFalse();
    }

    [Test]
    public async Task ShouldFilterByFullName()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test John Doe"));
        await AddAsync(CreatePerson("Test Jane Smith"));
        await AddAsync(CreatePerson("Test Bob Johnson"));

        var query = new GetPeopleWithPaginationQuery
        {
            FullName = "Smith",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().FullName.Should().Be("Test Jane Smith");
        result.TotalCount.Should().Be(1);
    }

    [Test]
    public async Task ShouldFilterByFullName_CaseInsensitive()
    {
        await ResetState();

        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test John Doe"));
        await AddAsync(CreatePerson("Test Jane Smith"));
        await AddAsync(CreatePerson("Test Bob Johnson"));

        var query = new GetPeopleWithPaginationQuery
        {
            FullName = "smith", // lowercase
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().HaveCount(1);
        result.Items.First().FullName.Should().Be("Test Jane Smith");
    }

    [Test]
    public async Task ShouldReturnCorrectPage_WhenPagingIsApplied()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test Person Page 1"));
        await AddAsync(CreatePerson("Test Person Page 2"));
        await AddAsync(CreatePerson("Test Person Page 3"));

        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 2,
            PageSize = 1
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().HaveCount(1);
        result.TotalCount.Should().Be(3);
        result.TotalPages.Should().Be(3);
        result.PageNumber.Should().Be(2);
        result.HasNextPage.Should().BeTrue();
        result.HasPreviousPage.Should().BeTrue();
    }

    [Test]
    public async Task ShouldReturnEmptyList_WhenPageNumberExceedsTotalPages()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test Person 1"));
        await AddAsync(CreatePerson("Test Person 2"));

        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 10,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(2);
    }

    [Test]
    public async Task ShouldReturnEmptyList_WhenNoMatchingRecordsFound()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test Person 1"));
        await AddAsync(CreatePerson("Test Person 2"));

        var query = new GetPeopleWithPaginationQuery
        {
            FullName = "NonExistentName",
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
    }

    [Test]
    public async Task ShouldFailValidation_WhenPageNumberIsLessThanOne()
    {
        // Arrange
        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 0,
            PageSize = 10
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenPageSizeIsLessThanOne()
    {
        // Arrange
        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 0
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldFailValidation_WhenFullNameExceedsMaxLength()
    {
        // Arrange
        var query = new GetPeopleWithPaginationQuery
        {
            FullName = new string('A', 71), // Max length is 70
            PageNumber = 1,
            PageSize = 10
        };

        // Act & Assert
        await FluentActions.Invoking(() =>
            SendAsync(query)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldIncludeAddressesAndPhoneNumbers_InReturnedPeople()
    {
        // Arrange - Create specific test data for this test only
        await AddAsync(CreatePerson("Test Person With Addresses 1"));
        await AddAsync(CreatePerson("Test Person With Addresses 2"));

        var query = new GetPeopleWithPaginationQuery
        {
            PageNumber = 1,
            PageSize = 10
        };

        // Act
        var result = await SendAsync(query);

        // Assert
        result.Items.Should().HaveCount(2);

        foreach (var person in result.Items)
        {
            person.Addresses.Should().HaveCount(2);

            var homeAddress = person.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Home);
            homeAddress.Should().NotBeNull();
            homeAddress!.PhoneNumbers.Should().NotBeEmpty();

            var businessAddress = person.Addresses.FirstOrDefault(a => a.AddressType == AddressType.Business);
            businessAddress.Should().NotBeNull();
            businessAddress!.PhoneNumbers.Should().NotBeEmpty();
        }
    }

    private Person CreatePerson(string fullName)
    {
        var homePhoneNumber = new PhoneNumber($"+1{new Random().Next(100000000, 999999999)}");
        var businessPhoneNumber = new PhoneNumber($"+2{new Random().Next(100000000, 999999999)}");

        var homeAddress = new Address(
            $"{new Random().Next(100, 999)} Home St",
            AddressType.Home,
            new List<PhoneNumber> { homePhoneNumber });

        var businessAddress = new Address(
            $"{new Random().Next(100, 999)} Business Ave",
            AddressType.Business,
            new List<PhoneNumber> { businessPhoneNumber });

        return new Person(
            fullName,
            homeAddress,
            businessAddress);
    }
}
