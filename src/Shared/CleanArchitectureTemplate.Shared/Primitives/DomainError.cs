namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents an error with a code, description, and type.
/// </summary>
public record DomainError
{
    /// <summary>
    /// Represents no error.
    /// </summary>
    public static readonly DomainError None = new(string.Empty, string.Empty, ErrorType.Failure);

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly DomainError NullValue = new(
        "General.Null",
        "Null value was provided",
        ErrorType.Failure);

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainError"/> record.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <param name="type">The type of the error.</param>
    public DomainError(string code, string description, ErrorType type)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Gets the error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the error description.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the type of the error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A failure <see cref="DomainError"/>.</returns>
    public static DomainError Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A not found <see cref="DomainError"/>.</returns>
    public static DomainError NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Creates a problem error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A problem <see cref="DomainError"/>.</returns>
    public static DomainError Problem(string code, string description) =>
        new(code, description, ErrorType.Problem);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A conflict <see cref="DomainError"/>.</returns>
    public static DomainError Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);
}
