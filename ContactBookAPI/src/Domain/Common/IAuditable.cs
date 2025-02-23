namespace ContactBookAPI.Domain.Common;

public interface IAuditable
{
    public void SetLastModifiedDetails(string? modifiedBy, TimeProvider timeProvider);
}
