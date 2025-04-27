using System.Diagnostics.CodeAnalysis;

namespace CleanArchitectureTemplate.Shared.Primitives;

/// <summary>
/// Represents the result of an operation, indicating success or failure.
/// </summary>
public class Result
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Result"/> class.
    /// </summary>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="errors">A collection of domain errors if the operation failed.</param>
    protected Result(bool isSuccess, IEnumerable<DomainError> errors)
    {
        IsSuccess = isSuccess;
        Errors = [.. errors];
    }

    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Gets a value indicating whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// Gets the list of domain errors if the operation failed.
    /// </summary>
    public List<DomainError> Errors { get; }

    /// <summary>
    /// Gets the primary error associated with the result.
    /// </summary>
    public DomainError Error => Errors.FirstOrDefault() ?? DomainError.None;

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A <see cref="Result"/> indicating success.</returns>
    public static Result Success() => new(true, []);

    /// <summary>
    /// Creates a failed result with a collection of domain errors.
    /// </summary>
    /// <param name="errors">The domain errors.</param>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure(IEnumerable<DomainError> errors) => new(false, errors);

    /// <summary>
    /// Creates a failed result with a single domain error.
    /// </summary>
    /// <param name="error">The domain error.</param>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure(DomainError error) => new(false, [error]);

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="Result"/> indicating failure.</returns>
    public static Result Failure(string code, string description) =>
        new(false, [DomainError.Failure(code, description)]);

    /// <summary>
    /// Maps the current result to a new result of type <typeparamref name="TValue"/>.
    /// </summary>
    /// <typeparam name="TValue">The type of the new result.</typeparam>
    /// <param name="func">A function to execute if the current result is successful.</param>
    /// <returns>A new <see cref="Result{TValue}"/>.</returns>
    public Result<TValue> Map<TValue>(Func<TValue> func)
    {
        return IsSuccess
            ? Result<TValue>.Success(func())
            : Result<TValue>.Failure(Errors);
    }

    /// <summary>
    /// Maps the current result to a new result of type <typeparamref name="TValue"/> using a function that returns a result.
    /// </summary>
    /// <typeparam name="TValue">The type of the new result.</typeparam>
    /// <param name="func">A function to execute if the current result is successful.</param>
    /// <returns>A new <see cref="Result{TValue}"/>.</returns>
    public Result<TValue> Map<TValue>(Func<Result<TValue>> func)
    {
        return IsSuccess
            ? func()
            : Result<TValue>.Failure(Errors);
    }
}

/// <summary>
/// Represents the result of an operation with a value, indicating success or failure.
/// </summary>
/// <typeparam name="TValue">The type of the value.</typeparam>
public class Result<TValue> : Result
{
    private readonly TValue? _value;

    /// <summary>
    /// Initializes a new instance of the <see cref="Result{TValue}"/> class.
    /// </summary>
    /// <param name="value">The value of the result if successful.</param>
    /// <param name="isSuccess">Indicates whether the operation was successful.</param>
    /// <param name="errors">A collection of domain errors if the operation failed.</param>
    protected Result(TValue? value, bool isSuccess, IEnumerable<DomainError> errors)
        : base(isSuccess, errors)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of the result if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if the result is a failure.</exception>
    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value of a failed result.");

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The value of the result.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating success.</returns>
    public static Result<TValue> Success(TValue value) => new(value, true, []);

    /// <summary>
    /// Creates a failed result with a collection of domain errors.
    /// </summary>
    /// <param name="errors">The domain errors.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating failure.</returns>
    public new static Result<TValue> Failure(IEnumerable<DomainError> errors) => new(default, false, errors);

    /// <summary>
    /// Creates a failed result with a single domain error.
    /// </summary>
    /// <param name="error">The domain error.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating failure.</returns>
    public new static Result<TValue> Failure(DomainError error) => new(default, false, [error]);

    /// <summary>
    /// Creates a failed result with a single error message.
    /// </summary>
    /// <param name="code">The error code.</param>
    /// <param name="description">The error description.</param>
    /// <returns>A <see cref="Result{TValue}"/> indicating failure.</returns>
    public new static Result<TValue> Failure(string code, string description) =>
        new(default, false, [DomainError.Failure(code, description)]);

    /// <summary>
    /// Creates a validation failure result.
    /// </summary>
    /// <param name="error">The validation error.</param>
    /// <returns>A failed <see cref="Result{TValue}"/> with a validation error.</returns>
    public static Result<TValue> ValidationFailure(DomainError error) =>
        new(default, false, [error]);

    /// <summary>
    /// Maps the current result to a new result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <param name="func">A function to execute if the current result is successful.</param>
    /// <returns>A new <see cref="Result{TResult}"/>.</returns>
    public Result<TResult> Map<TResult>(Func<TValue, TResult> func)
    {
        return IsSuccess
            ? Result<TResult>.Success(func(Value))
            : Result<TResult>.Failure(Errors);
    }

    /// <summary>
    /// Maps the current result to a new result of type <typeparamref name="TResult"/> using a function that returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the new result.</typeparam>
    /// <param name="func">A function to execute if the current result is successful.</param>
    /// <returns>A new <see cref="Result{TResult}"/>.</returns>
    public Result<TResult> Map<TResult>(Func<TValue, Result<TResult>> func)
    {
        return IsSuccess
            ? func(Value)
            : Result<TResult>.Failure(Errors);
    }

    /// <summary>
    /// Implicitly converts a value to a successful result.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    public static implicit operator Result<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure(DomainError.NullValue);
}