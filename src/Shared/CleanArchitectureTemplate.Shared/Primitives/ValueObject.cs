namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents a base class for value objects in the domain.
/// Value objects are immutable and are compared based on their properties' values.
/// </summary>
public abstract record ValueObject
{
    /// <summary>
    /// Creates a new instance of a value object, performing the specified validations.
    /// </summary>
    /// <typeparam name="T">The type of the value object being created.</typeparam>
    /// <param name="factory">A factory function to create the value object instance.</param>
    /// <param name="validations">A collection of validation rules, each containing a boolean indicating validity and a domain error.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created value object if all validations pass,
    /// or a failure result with the list of domain errors.
    /// </returns>
    protected static Result<T> Create<T>(Func<T> factory, params (bool IsValid, DomainError Error)[] validations)
        where T : ValueObject
    {
        var failures = validations
            .Where(v => !v.IsValid)
            .Select(v => v.Error)
            .ToList();

        return failures.Count != 0
            ? Result<T>.Failure(failures)
            : Result<T>.Success(factory());
    }

    /// <summary>
    /// Creates a new instance of a value object, performing the specified validations with string error messages.
    /// </summary>
    /// <typeparam name="T">The type of the value object being created.</typeparam>
    /// <param name="factory">A factory function to create the value object instance.</param>
    /// <param name="validations">A collection of validation rules, each containing a boolean indicating validity and an error message.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created value object if all validations pass,
    /// or a failure result with domain errors created from the error messages.
    /// </returns>
    protected static Result<T> Create<T>(Func<T> factory, params (bool IsValid, string ErrorCode, string ErrorMessage)[] validations)
        where T : ValueObject
    {
        var failures = validations
            .Where(v => !v.IsValid)
            .Select(v => DomainError.Failure(v.ErrorCode, v.ErrorMessage))
            .ToList();

        return failures.Count != 0
            ? Result<T>.Failure(failures)
            : Result<T>.Success(factory());
    }

    /// <summary>
    /// Creates a new instance of a value object, performing the specified validations with a custom error type.
    /// </summary>
    /// <typeparam name="T">The type of the value object being created.</typeparam>
    /// <param name="factory">A factory function to create the value object instance.</param>
    /// <param name="validations">A collection of validation rules, each containing a boolean indicating validity, error code, message and type.</param>
    /// <returns>
    /// A <see cref="Result{T}"/> containing the created value object if all validations pass,
    /// or a failure result with domain errors of specified types.
    /// </returns>
    protected static Result<T> Create<T>(Func<T> factory, params (bool IsValid, string ErrorCode, string ErrorMessage, ErrorType ErrorType)[] validations)
        where T : ValueObject
    {
        var failures = validations
            .Where(v => !v.IsValid)
            .Select(v => new DomainError(v.ErrorCode, v.ErrorMessage, v.ErrorType))
            .ToList();

        return failures.Count != 0
            ? Result<T>.Failure(failures)
            : Result<T>.Success(factory());
    }
}