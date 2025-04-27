namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents the base class for all entities in the domain.
/// </summary>
/// <typeparam name="TId">The type of the entity's identifier.</typeparam>
/// <typeparam name="TUserId">The type of user identifier for auditing.</typeparam>
public abstract class EntityBase<TId, TUserId>
    where TId : EntityId<Guid>
    where TUserId : UserId<Guid>
{
    /// <summary>
    /// Gets the unique identifier of the entity.
    /// </summary>
    public TId Id { get; protected set; } = default!;

    /// <summary>
    /// Gets or sets the timestamp when the entity was created.
    /// </summary>
    public CreationTime CreatedOn { get; set; } = default!;

    /// <summary>
    /// Gets or sets the identifier of the user who created the entity.
    /// </summary>
    public TUserId CreatedBy { get; set; } = default!;

    /// <summary>
    /// Gets or sets the timestamp when the entity was last modified.
    /// </summary>
    public ModificationTime ModifiedOn { get; set; } = default!;

    /// <summary>
    /// Gets or sets the identifier of the user who last modified the entity.
    /// </summary>
    public TUserId ModifiedBy { get; set; } = default!;

    /// <summary>
    /// Determines whether two entities are equal by comparing their identifiers.
    /// </summary>
    /// <param name="obj">The object to compare with the current entity.</param>
    /// <returns><c>true</c> if the specified object is equal to the current entity; otherwise, <c>false</c>.</returns>
    public override bool Equals(object? obj)
    {
        if (obj is not EntityBase<TId, TUserId> other)
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
