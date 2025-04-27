namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a validation error that contains multiple errors.
/// </summary>
public sealed record ValidationError : DomainError
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationError"/> record.
    /// </summary>
    /// <param name="errors">The collection of validation errors.</param>
    public ValidationError(DomainError[] errors)
        : base(
            "Validation.General",
            "One or more validation errors occurred",
            ErrorType.Validation)
    {
        Errors = errors;
    }

    /// <summary>
    /// Gets the collection of validation errors.
    /// </summary>
    public DomainError[] Errors { get; }

    /// <summary>
    /// Creates a <see cref="ValidationError"/> from a collection of <see cref="Result"/> objects.
    /// </summary>
    /// <param name="results">The collection of results to extract errors from.</param>
    /// <returns>A <see cref="ValidationError"/> containing the extracted errors.</returns>
    public static ValidationError FromResults(IEnumerable<Result> results) =>
        new([.. results.Where(r => r.IsFailure).Select(r => r.Error)]);
}
