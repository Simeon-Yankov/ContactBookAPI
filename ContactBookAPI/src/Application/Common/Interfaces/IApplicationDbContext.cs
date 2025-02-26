using ContactBookAPI.Domain.Entities;

namespace ContactBookAPI.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Person> People { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
