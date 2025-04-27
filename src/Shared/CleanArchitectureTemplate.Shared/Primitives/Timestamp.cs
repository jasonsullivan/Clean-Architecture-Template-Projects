namespace CleanArchitectureTemplate.Shared.Primitives;


/// <summary>
/// Represents a base value object for timestamps.
/// </summary>
public abstract record Timestamp : ValueObject
{
    /// <summary>
    /// Gets the underlying <see cref="DateTime"/> value of the timestamp.
    /// </summary>
    public DateTime Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Timestamp"/> class.
    /// </summary>
    /// <param name="value">The timestamp value.</param>
    protected Timestamp(DateTime value) => Value = value;

    /// <summary>
    /// Creates a new timestamp with validation.
    /// </summary>
    /// <typeparam name="T">The type of the timestamp being created.</typeparam>
    /// <param name="timestamp">The <see cref="DateTime"/> value to create the timestamp from.</param>
    /// <param name="factory">A factory function to create the timestamp instance.</param>
    /// <returns>A <see cref="Result{T}"/> containing the timestamp or validation errors.</returns>
    protected static Result<T> CreateTimestamp<T>(DateTime timestamp, Func<DateTime, T> factory)
        where T : Timestamp
    {
        return Create(
            () => factory(timestamp),
            (
                timestamp != default,
                "Timestamp.Default",
                "Timestamp cannot be the default value."
            ),
            (
                timestamp.Kind != DateTimeKind.Unspecified,
                "Timestamp.Unspecified",
                "Timestamp must specify UTC or Local kind."
            )
        );
    }

    /// <summary>
    /// Implicitly converts a <see cref="Timestamp"/> to its underlying <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="timestamp">The timestamp to convert.</param>
    public static implicit operator DateTime(Timestamp timestamp) => timestamp.Value;
}
