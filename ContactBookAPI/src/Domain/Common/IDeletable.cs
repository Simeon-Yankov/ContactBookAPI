namespace ContactBookAPI.Domain.Common;

public interface IDeletable
{
    public void Delete(string? deletedBy, TimeProvider timeProvider);
}
