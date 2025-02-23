using ContactBookAPI.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace ContactBookAPI.Infrastructure.Data.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly TimeProvider _dateTime;

    public AuditableEntityInterceptor(
        TimeProvider dateTime)
    {
        _dateTime = dateTime;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var utcNow = _dateTime.GetUtcNow();

        AddEntities(eventData.Context, utcNow);
        UpdateEntities(eventData.Context, utcNow);
        DeleteEntities(eventData.Context, utcNow);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        var utcNow = _dateTime.GetUtcNow();

        AddEntities(eventData.Context, utcNow);
        UpdateEntities(eventData.Context, utcNow);
        DeleteEntities(eventData.Context, utcNow);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void AddEntities(DbContext? context, DateTimeOffset utcNow)
    {
        if (context == null) return;

        var addedEntities = context.ChangeTracker.Entries<BaseEntity>()
                .Where(e => e.State == EntityState.Added)
                .ToList();

        foreach (var entry in addedEntities)
        {
            entry.Entity.SetCreationDetails(null, utcNow);
        }
    }

    public void UpdateEntities(DbContext? context, DateTimeOffset utcNow)
    {
        if (context == null) return;

        var auditedEntities = context.ChangeTracker.Entries<IAuditable>()
                .Where(e => e.State == EntityState.Modified || e.HasChangedOwnedEntities())
                .ToList();

        foreach (var entry in auditedEntities)
        {
            entry.Entity.SetLastModifiedDetails(null, _dateTime);
        }
    }

    /// <summary>
    /// Soft Delete
    /// </summary>
    /// <param name="context"></param>
    /// <param name="utcNow"></param>
    public void DeleteEntities(DbContext? context, DateTimeOffset utcNow)
    {
        if (context == null) return;

        var softDeletableEntitiesOnDelete = context.ChangeTracker.Entries<IDeletable>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

        foreach (var entry in softDeletableEntitiesOnDelete)
        {
            entry.State = EntityState.Modified;
            entry.Entity.Delete(null, _dateTime);
        }
    }
}

public static class Extensions
{
    public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
        entry.References.Any(r =>
            r.TargetEntry != null &&
            r.TargetEntry.Metadata.IsOwned() &&
            (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
}
