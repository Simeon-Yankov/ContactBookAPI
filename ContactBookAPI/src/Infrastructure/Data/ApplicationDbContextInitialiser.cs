using Bogus;
using ContactBookAPI.Domain.Enums;
using ContactBookAPI.Domain.ValueObjects;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ContactBookAPI.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();

        await initialiser.SeedAsync();
    }
}

public class ApplicationDbContextInitialiser
{
    private readonly ILogger<ApplicationDbContextInitialiser> _logger;
    private readonly ApplicationDbContext _context;

    public ApplicationDbContextInitialiser(ILogger<ApplicationDbContextInitialiser> logger, ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        if (_context.People.Any())
        {
            _logger.LogInformation("Database already contains person data - skipping seeding");
            return;
        }

        _logger.LogInformation("Seeding database...");

        Faker.GlobalUniqueIndex = 0;

        var phoneNumberFaker = new Faker<PhoneNumber>()
            .CustomInstantiator(f => new PhoneNumber(
                $"+{f.Random.Number(1, 9)}{f.Random.Number(100000000, 999999999)}"
            ));

        var homeAddressFaker = new Faker<Address>()
            .CustomInstantiator(f => {
                var phoneNumbers = phoneNumberFaker.Generate(f.Random.Number(1, 3));

                return new Address(
                    $"{f.Address.StreetAddress()}, {f.Address.City()}, {f.Address.StateAbbr()} {f.Address.ZipCode()}",
                    AddressType.Home,
                    phoneNumbers
                );
            });

        var businessAddressFaker = new Faker<Address>()
            .CustomInstantiator(f => {
                var phoneNumbers = phoneNumberFaker.Generate(f.Random.Number(1, 2));

                return new Address(
                    $"{f.Company.CompanyName()}, {f.Address.StreetAddress()}, {f.Address.City()}, {f.Address.StateAbbr()} {f.Address.ZipCode()}",
                    AddressType.Business,
                    phoneNumbers
                );
            });

        var personFaker = new Faker<ContactBookAPI.Domain.Entities.Person>()
            .CustomInstantiator(f => {
                var fullName = $"{f.Name.FirstName()} {f.Name.LastName()}";
                var homeAddress = homeAddressFaker.Generate();
                var businessAddress = businessAddressFaker.Generate();

                return new ContactBookAPI.Domain.Entities.Person(
                    fullName,
                    homeAddress,
                    businessAddress
                );
            });

        var people = personFaker.Generate(4);

        await _context.People.AddRangeAsync(people);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Seeded 4 sample people with addresses and phone numbers");
    }
}
