namespace CleanArchitectureTemplate.Shared.Primitives;


/// <summary>
/// Represents a base class for domain events in the application.
/// </summary>
public abstract class DomainEvent : IDomainEvent
{
    /// <summary>
    /// Gets the unique identifier for the domain event.
    /// </summary>
    public Guid EventId { get; }

    /// <summary>
    /// Gets the date and time when the domain event occurred, in UTC.
    /// </summary>
    public DateTime OccurredOn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainEvent"/> class.
    /// </summary>
    protected DomainEvent()
    {
        EventId = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}