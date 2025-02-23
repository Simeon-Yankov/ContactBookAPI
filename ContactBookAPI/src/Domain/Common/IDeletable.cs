namespace ContactBookAPI.Domain.Common;

public interface IDeletable
{
    void Delete(string? deletedBy, TimeProvider timeProvider);
    void Delete(string? deletedBy, DateTimeOffset utcNow);
}
