﻿namespace ContactBookAPI.Domain.Common;

public abstract class BaseDeletableAuditableEntity : BaseAuditableEntity, IDeletable, IAuditable
{
    public bool IsDeleted { get; private set; }
    public string? DeletedBy { get; private set; }
    public DateTimeOffset? Deleted { get; private set; }

    public void Delete(string? deletedBy, TimeProvider timeProvider)
        => Delete(deletedBy, timeProvider?.GetUtcNow() ?? DateTime.UtcNow);

    public void Delete(string? deletedBy, DateTimeOffset utcNow)
    {
        IsDeleted = true;
        DeletedBy = deletedBy;
        Deleted = utcNow;
    }
}
