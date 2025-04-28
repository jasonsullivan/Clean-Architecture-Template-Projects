using CleanArchitectureTemplate.Shared.Primitives;

using System.Text.RegularExpressions;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a phone number value object in the domain.
/// </summary>
public sealed partial record PhoneNumber : ValueObject
{
    /// <summary>
    /// Gets the phone number value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PhoneNumber"/> class with the specified phone number value.
    /// </summary>
    /// <param name="value">The phone number value.</param>
    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="PhoneNumber"/> instance after validating the provided phone number string.
    /// </summary>
    /// <param name="value">The phone number string to validate and create the value object from.</param>
    /// <returns>
    /// A <see cref="Result{PhoneNumber}"/> containing the created <see cref="PhoneNumber"/> instance if valid,
    /// or a failure result with the appropriate domain error.
    /// </returns>
    public static Result<PhoneNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PhoneNumber>.Failure(DomainError.Failure("PhoneNumber", "Phone number cannot be empty."));
        }
        if (!PhoneNumberValidationRegex().IsMatch(value))
        {
            return Result<PhoneNumber>.Failure(DomainError.Failure("PhoneNumber", "Invalid phone number format."));
        }
        return Result<PhoneNumber>.Success(new PhoneNumber(value));
    }

    /// <summary>
    /// Provides the regular expression used to validate phone numbers.
    /// </summary>
    /// <returns>A <see cref="Regex"/> instance for phone number validation.</returns>
    [GeneratedRegex(@"^\+?[1-9]\d{1,14}$")]
    private static partial Regex PhoneNumberValidationRegex();
}
