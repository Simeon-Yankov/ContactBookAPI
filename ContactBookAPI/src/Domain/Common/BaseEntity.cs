using System.ComponentModel.DataAnnotations.Schema;

namespace ContactBookAPI.Domain.Common;

public abstract class BaseEntity : IEntity
{
    /// <summary>
    /// EF Core requires a parameterless constructor to instantiate entities.
    /// </summary>
    protected BaseEntity()
    {
    }

    public int Id { get; private set; }

    public DateTimeOffset Created { get; private set; }
    
    public string? CreatedBy { get; private set; }


    private readonly List<BaseEvent> _domainEvents = new();

    [NotMapped]
    public IReadOnlyCollection<BaseEvent> DomainEvents => _domainEvents.AsReadOnly();

    public void SetCreationDetails(string? createdBy, TimeProvider timeProvider)
    {
        this.CreatedBy = createdBy;
        this.Created = timeProvider?.GetUtcNow() ?? DateTime.UtcNow;
    }

    public void SetCreationDetails(string? createdBy, DateTimeOffset createdUtc)
    {
        this.CreatedBy = createdBy;
        this.Created = createdUtc;
    }

    public void AddDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(BaseEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
