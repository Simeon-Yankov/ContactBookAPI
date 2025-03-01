using System.Data.Common;
using ContactBookAPI.Infrastructure.Data;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Testcontainers.PostgreSql;

namespace ContactBookAPI.Application.FunctionalTests;

public class TestcontainersTestDatabase : ITestDatabase
{
    private readonly PostgreSqlContainer _container;
    private DbConnection _connection = null!;
    private string _connectionString = null!;
    private Respawner _respawner = null!;

    public TestcontainersTestDatabase()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithAutoRemove(true)
            .Build();
    }

    public async Task InitialiseAsync()
    {
        await _container.StartAsync();

        _connectionString = _container.GetConnectionString();

        _connection = new NpgsqlConnection(_connectionString);

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_connection)
            .Options;

        await _connection.OpenAsync();

        var context = new ApplicationDbContext(options);

        await context.Database.MigrateAsync();

        _respawner = await Respawner.CreateAsync(_connection, new RespawnerOptions
        {
            TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
            DbAdapter = DbAdapter.Postgres,
        });
    }

    public DbConnection GetConnection()
    {
        return _connection;
    }

    public async Task ResetAsync()
    {

        // If respawner fails, try to recreate it
        //_respawner = await Respawner.CreateAsync(_connectionString, new RespawnerOptions
        //{
        //    TablesToIgnore = new Respawn.Graph.Table[] { "__EFMigrationsHistory" },
        //    DbAdapter = DbAdapter.Postgres,
        //    SchemasToInclude = new[] { "public" }
        //});

        await _respawner.ResetAsync(_connection);
    }

    public async Task DisposeAsync()
    {
        await _connection.DisposeAsync();
        await _container.DisposeAsync();
    }
}
