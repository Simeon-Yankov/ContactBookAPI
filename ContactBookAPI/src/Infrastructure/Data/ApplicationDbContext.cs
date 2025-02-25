using System.Reflection;
using ContactBookAPI.Application.Common.Interfaces;
using ContactBookAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ContactBookAPI.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Person> People => Set<Person>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
