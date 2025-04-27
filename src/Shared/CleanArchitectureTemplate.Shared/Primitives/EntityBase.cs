namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents the base class for all entities in the domain.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <remarks>
/// Initializes a new instance of the <see cref="EntityBase{TId}"/> class with the specified identifier.
/// </remarks>
/// <param name="id">The identifier for the entity.</param>
public abstract class EntityBase<TId>(TId id)
    where TId : EntityId<Guid>
{

    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public TId Id { get; protected set; } = id;

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    public DateTime CreatedOn { get; set; } = default!;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public Guid CreatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    public DateTime ModifiedOn { get; set; } = default!;

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public Guid ModifiedBy { get; set; } = default!;

    /// <summary>
    /// Determines whether two entities are equal by comparing their identifiers.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns><c>true</c> if the specified object is equal to the current entity; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TId> other)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id.Equals(other.Id);
    }

    /// <summary>
    /// Returns a hash code for the current entity.
    /// </summary>
    /// <returns>A hash code for the current entity.</returns>
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}
