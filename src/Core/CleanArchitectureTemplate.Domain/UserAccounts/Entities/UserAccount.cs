using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

public sealed class UserAccount : EntityBase<UserAccountId, UserAccountId>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    public UserAccount()
    {
        // Constructor logic can be added here if needed
    }
    /// <summary>
    /// Gets or sets the username of the user account.
    /// </summary>
    public string Username { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the email address associated with the user account.
    /// </summary>
    public string Email { get; set; } = string.Empty;
    // Additional properties and methods can be added here as needed
}