using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Infrastructure.Common;

/// <summary>
/// Provides the current date and time information for the application.
/// Implements the <see cref="IDateTimeProvider"/> interface to allow for easier testing and abstraction.
/// </summary>
public class SystemDateTimeProvider : IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    public DateTime UtcNow => DateTime.UtcNow;

    /// <summary>
    /// Gets the current local date and time.
    /// </summary>
    public DateTime Now => DateTime.Now;

    /// <summary>
    /// Gets the current UTC date and time as a <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeOffset UtcNowOffset => DateTimeOffset.UtcNow;

    /// <summary>
    /// Gets the current local date and time as a <see cref="DateTimeOffset"/>.
    /// </summary>
    public DateTimeOffset NowOffset => DateTimeOffset.Now;

    /// <summary>
    /// Gets the current date with the time component set to 00:00:00.
    /// </summary>
    public DateTime Today => DateTime.Today;
}
