using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents the status of a user account.
/// </summary>
public sealed record UserStatus : ValueObject
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserStatus"/> record.
    /// </summary>
    /// <param name="value">The string value of the user status.</param>
    private UserStatus(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the string value of the user status.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Represents an active user status.
    /// </summary>
    public static UserStatus Active => new("Active");

    /// <summary>
    /// Represents an inactive user status.
    /// </summary>
    public static UserStatus Inactive => new("Inactive");

    /// <summary>
    /// Represents a locked user status.
    /// </summary>
    public static UserStatus Locked => new("Locked");

    /// <summary>
    /// Represents a pending activation user status.
    /// </summary>
    public static UserStatus PendingActivation => new("PendingActivation");

    /// <summary>
    /// Creates a <see cref="UserStatus"/> instance from a string value.
    /// </summary>
    /// <param name="status">The string representation of the user status.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="UserStatus"/> if successful,
    /// or a failure result with a <see cref="DomainError"/> if the status is invalid.
    /// </returns>
    public static Result<UserStatus> Create(string status)
    {
        if (string.IsNullOrWhiteSpace(status))
        {
            return Result<UserStatus>.Failure(DomainError.Failure("UserStatus", "Status cannot be empty."));
        }

        var normalizedStatus = status.Trim();

        return normalizedStatus switch
        {
            "Active" => Result<UserStatus>.Success(Active),
            "Inactive" => Result<UserStatus>.Success(Inactive),
            "Locked" => Result<UserStatus>.Success(Locked),
            "PendingActivation" => Result<UserStatus>.Success(PendingActivation),
            _ => Result<UserStatus>.Failure(DomainError.Failure("UserStatus", $"Invalid user status: {status}"))
        };
    }

    /// <summary>
    /// Gets a value indicating whether the user status is active.
    /// </summary>
    public bool IsActive => string.Equals(Value, Active.Value, StringComparison.Ordinal);

    /// <summary>
    /// Gets a value indicating whether the user status is inactive.
    /// </summary>
    public bool IsInactive => string.Equals(Value, Inactive.Value, StringComparison.Ordinal);

    /// <summary>
    /// Gets a value indicating whether the user status is locked.
    /// </summary>
    public bool IsLocked => string.Equals(Value, Locked.Value, StringComparison.Ordinal);

    /// <summary>
    /// Gets a value indicating whether the user status is pending activation.
    /// </summary>
    public bool IsPendingActivation => string.Equals(Value, PendingActivation.Value, StringComparison.Ordinal);
}
