namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Provides date and time information for the application.
/// This abstraction makes it easier to test time-dependent code.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    DateTime Now { get; }

    /// <summary>
    /// Gets the current UTC date and time as a DateTimeOffset.
    /// </summary>
    DateTimeOffset UtcNowOffset { get; }

    /// <summary>
    /// Gets the current local date and time as a DateTimeOffset.
    /// </summary>
    DateTimeOffset NowOffset { get; }

    /// <summary>
    /// Gets the current date.
    /// </summary>
    DateTime Today { get; }
}
