using ContactBookAPI.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Data;

namespace ContactBookAPI.Application.FunctionalTests;

[SetUpFixture]
public partial class Testing
{
    private static ITestDatabase _database;
    private static CustomWebApplicationFactory _factory = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static IServiceScope _rootScope = null!; // Add this for long-lived services

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        _database = await TestDatabaseFactory.CreateAsync();

        _factory = new CustomWebApplicationFactory(_database.GetConnection());

        _scopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();

        // Create a root scope that will live for the duration of all tests
        _rootScope = _scopeFactory.CreateScope();

        // Ensure the connection is open
        var dbConnection = _rootScope.ServiceProvider.GetRequiredService<IDbConnection>();
        if (dbConnection.State != ConnectionState.Open)
        {
            dbConnection.Open();
        }
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        // Use the root scope for database operations
        var mediator = _rootScope.ServiceProvider.GetRequiredService<ISender>();
        return await mediator.Send(request);
    }

    public static async Task SendAsync(IBaseRequest request)
    {
        // Use the root scope for database operations
        var mediator = _rootScope.ServiceProvider.GetRequiredService<ISender>();
        await mediator.Send(request);
    }

    public static async Task ResetState()
    {
        try
        {
            await _database.ResetAsync();

            // After resetting the database, ensure the connection is still open
            var dbConnection = _rootScope.ServiceProvider.GetRequiredService<IDbConnection>();
            if (dbConnection.State != ConnectionState.Open)
            {
                dbConnection.Open();
            }
        }
        catch (Exception)
        {
        }
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        // Use the root scope for database operations
        var context = _rootScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        // Use the root scope for database operations
        var context = _rootScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Add(entity);
        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        // Use the root scope for database operations
        var context = _rootScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public async Task RunAfterAnyTests()
    {
        // Dispose the root scope first
        if (_rootScope != null)
        {
            _rootScope.Dispose();
        }

        await _database.DisposeAsync();
        await _factory.DisposeAsync();
    }
}
