using CleanArchitectureTemplate.Shared.Primitives;

using System.Text.RegularExpressions;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a username value object in the domain.
/// </summary>
public sealed partial record UserName : ValueObject
{
    /// <summary>
    /// Gets the username value.
    /// </summary>
    public string Value { get; init; } = string.Empty;

    /// <summary>
    /// Creates a new <see cref="UserName"/> instance after validating the provided username string.
    /// </summary>
    /// <param name="value">The username string to validate and create the value object from.</param>
    /// <returns>
    /// A <see cref="Result{UserName}"/> containing the created <see cref="UserName"/> instance if valid,
    /// or a failure result with the appropriate domain error.
    /// </returns>
    public static Result<UserName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<UserName>.Failure(DomainError.Failure("UserName", "UserName cannot be empty."));
        }
        if (value.Length < 3 || value.Length > 20)
        {
            return Result<UserName>.Failure(DomainError.Failure("UserName", "UserName must be between 3 and 20 characters."));
        }
        if (!UserNameValidationRegex().IsMatch(value))
        {
            return Result<UserName>.Failure(DomainError.Failure("UserName", "UserName can only contain letters, numbers, and underscores."));
        }
        return Result<UserName>.Success(new UserName { Value = value });
    }

    /// <summary>
    /// Provides the regular expression used to validate usernames.
    /// </summary>
    /// <returns>A <see cref="Regex"/> instance for username validation.</returns>
    [GeneratedRegex(@"^[a-zA-Z0-9_]+$")]
    private static partial Regex UserNameValidationRegex();
}
