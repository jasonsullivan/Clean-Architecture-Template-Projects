namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a modification timestamp for domain entities.
/// </summary>
public record ModificationTime : Timestamp
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModificationTime"/> class.
    /// </summary>
    /// <param name="value">The <see cref="DateTime"/> value of the modification timestamp.</param>
    private ModificationTime(DateTime value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="ModificationTime"/> from a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="timestamp">The <see cref="DateTime"/> value to create the modification timestamp from.</param>
    /// <returns>A <see cref="Result{ModificationTime}"/> containing the modification timestamp or validation errors.</returns>
    public static Result<ModificationTime> Create(DateTime timestamp)
    {
        return CreateTimestamp(timestamp, dt => new ModificationTime(dt));
    }

    /// <summary>
    /// Creates a new <see cref="ModificationTime"/> with the current UTC time.
    /// </summary>
    /// <param name="dateTimeProvider">The provider to get the current UTC time.</param>
    /// <returns>A <see cref="Result{ModificationTime}"/> containing the modification timestamp or validation errors.</returns>
    public static Result<ModificationTime> CreateUtcNow(IDateTimeProvider dateTimeProvider)
    {
        return Create(dateTimeProvider.UtcNow);
    }
}
