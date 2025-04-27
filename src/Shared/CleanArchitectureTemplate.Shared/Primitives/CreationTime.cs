namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a creation timestamp for domain entities.
/// </summary>
public record CreationTime : Timestamp
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreationTime"/> class.
    /// </summary>
    /// <param name="value">The <see cref="DateTime"/> value of the creation timestamp.</param>
    private CreationTime(DateTime value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="CreationTime"/> from a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="timestamp">The <see cref="DateTime"/> value to create the creation timestamp from.</param>
    /// <returns>A <see cref="Result{CreationTime}"/> containing the creation timestamp or validation errors.</returns>
    public static Result<CreationTime> Create(DateTime timestamp)
    {
        return CreateTimestamp(timestamp, dt => new CreationTime(dt));
    }

    /// <summary>
    /// Creates a new <see cref="CreationTime"/> with the current UTC time.
    /// </summary>
    /// <param name="dateTimeProvider">The provider to get the current UTC time.</param>
    /// <returns>A <see cref="Result{CreationTime}"/> containing the creation timestamp or validation errors.</returns>
    public static Result<CreationTime> CreateUtcNow(IDateTimeProvider dateTimeProvider)
    {
        return Create(dateTimeProvider.UtcNow);
    }
}
