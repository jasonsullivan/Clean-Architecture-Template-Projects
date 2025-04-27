namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents the type of an error.
/// </summary>
public enum ErrorType
{
    /// <summary>
    /// Represents a general failure.
    /// </summary>
    Failure = 0,

    /// <summary>
    /// Represents a validation error.
    /// </summary>
    Validation = 1,

    /// <summary>
    /// Represents a problem error.
    /// </summary>
    Problem = 2,

    /// <summary>
    /// Represents a not found error.
    /// </summary>
    NotFound = 3,

    /// <summary>
    /// Represents a conflict error.
    /// </summary>
    Conflict = 4
}
