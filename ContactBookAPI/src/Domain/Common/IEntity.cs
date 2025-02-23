namespace ContactBookAPI.Domain.Common;

internal interface IEntity
{
    public void SetCreationDetails(string? createdBy, TimeProvider timeProvider);
}
