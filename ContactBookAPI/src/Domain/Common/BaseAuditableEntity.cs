namespace ContactBookAPI.Domain.Common;

public abstract class BaseAuditableEntity : BaseEntity, IAuditable
{
    public DateTimeOffset? LastModified { get; private set; }

    public string? LastModifiedBy { get; private set; }

    public void SetLastModifiedDetails(string? modifiedBy, TimeProvider timeProvider)
    {
        this.LastModifiedBy = modifiedBy;
        this.LastModified = timeProvider?.GetUtcNow() ?? DateTime.UtcNow;
    }

    public void SetLastModifiedDetails(string? modifiedBy, DateTimeOffset lastModifiedUtc)
    {
        this.LastModifiedBy = modifiedBy;
        this.LastModified = lastModifiedUtc;
    }
}
