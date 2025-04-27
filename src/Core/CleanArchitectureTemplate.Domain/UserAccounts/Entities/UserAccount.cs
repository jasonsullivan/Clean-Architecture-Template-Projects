using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public sealed class UserAccount : AggregateRoot<UserAccountId>
{
    /// <summary>
    /// Gets the username of the user account.
    /// </summary>
    public UserName Username { get; private set; }

    /// <summary>
    /// Gets the email address associated with the user account.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the current status of the user account.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user account.</param>
    /// <param name="username">The username of the user account.</param>
    /// <param name="email">The email address associated with the user account.</param>
    /// <param name="status">The current status of the user account.</param>
    private UserAccount(UserAccountId id, UserName username, Email email, UserStatus status) : base(id)
    {
        Username = username;
        Email = email;
        Status = status;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user account.</param>
    /// <param name="username">The username of the user account.</param>
    /// <param name="email">The email address associated with the user account.</param>
    /// <param name="status">The current status of the user account.</param>
    /// <returns>A new <see cref="UserAccount"/> instance.</returns>
    public static UserAccount Create(UserAccountId id, UserName username, Email email, UserStatus status)
    {
        return new UserAccount(id, username, email, status);
    }
}
