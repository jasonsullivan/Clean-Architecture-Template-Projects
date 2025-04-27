namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a user identifier.
/// </summary>
/// <typeparam name="T">The type of the user identifier value.</typeparam>
public abstract record UserId<T> : EntityId<T> where T : notnull
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserId{T}"/> class.
    /// </summary>
    /// <param name="value">The value of the user identifier.</param>
    protected UserId(T value) : base(value) { }
}
