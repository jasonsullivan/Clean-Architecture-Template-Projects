using CleanArchitectureTemplate.Shared.Primitives;

using System.Text.RegularExpressions;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents an email value object in the domain.
/// </summary>
public sealed partial record Email : ValueObject
{
    /// <summary>
    /// Gets the email value.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new <see cref="Email"/> instance after validating the provided email string.
    /// </summary>
    /// <param name="value">The email string to validate and create the value object from.</param>
    /// <returns>
    /// A <see cref="Result{Email}"/> containing the created <see cref="Email"/> instance if valid,
    /// or a failure result with the appropriate domain error.
    /// </returns>
    public static Result<Email> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Email>.Failure(DomainError.Failure("Email", "Email cannot be empty."));
        }

        if (!EmailValidationRegex().IsMatch(value))
        {
            return Result<Email>.Failure(DomainError.Failure("Email", "Invalid email format."));
        }

        return Result<Email>.Success(new Email { Value = value });
    }

    /// <summary>
    /// Provides the regular expression used to validate email addresses.
    /// </summary>
    /// <returns>A <see cref="Regex"/> instance for email validation.</returns>
    [GeneratedRegex(@"^(?:[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?)*$")]
    private static partial Regex EmailValidationRegex();
}
