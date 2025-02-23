namespace ContactBookAPI.Domain.Common;

public interface IAuditable
{
    void SetLastModifiedDetails(string? modifiedBy, TimeProvider timeProvider);
    void SetLastModifiedDetails(string? modifiedBy, DateTimeOffset lastModifiedUtc);
}
