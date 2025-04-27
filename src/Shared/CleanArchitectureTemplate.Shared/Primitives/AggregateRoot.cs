namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents the base class for aggregate roots in the domain.
/// </summary>
/// <typeparam name="TId">The type of the aggregate root's identifier.</typeparam>
public abstract class AggregateRoot<TId>(TId id) : EntityBase<TId>(id)
    where TId : EntityId<Guid>
{
    /// <summary>
    /// A collection of domain events associated with the aggregate root.
    /// </summary>
    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>
    /// Gets the read-only collection of domain events.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to the aggregate root.
    /// </summary>
    /// <param name="domainEvent">The domain event to add.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Clears all domain events from the aggregate root.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
