namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a base class for entity identifiers with a strongly-typed value.
/// </summary>
/// <typeparam name="T">The type of the identifier value. Must be non-nullable.</typeparam>
public abstract record EntityId<T>(T Value) where T : notnull
{
    /// <summary>
    /// Implicitly converts an <see cref="EntityId{T}"/> to its underlying value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="id">The <see cref="EntityId{T}"/> instance to convert.</param>
    public static implicit operator T(EntityId<T> id) => id.Value;

    /// <summary>
    /// Returns the string representation of the identifier value.
    /// </summary>
    /// <returns>The string representation of the <see cref="Value"/> property, or an empty string if <see cref="Value"/> is null.</returns>
    public override string ToString() => Value?.ToString() ?? string.Empty;
}
