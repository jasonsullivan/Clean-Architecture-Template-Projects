using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents an application user with extended properties.
/// </summary>
public class ApplicationUser : IdentityUser
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUser"/> class.
    /// </summary>
    public ApplicationUser() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUser"/> class with a specified username.
    /// </summary>
    /// <param name="userName">The username for the application user.</param>
    public ApplicationUser(string userName) : base(userName)
    {
    }

    /// <summary>
    /// Gets or sets the first name of the user.
    /// </summary>
    public string? FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the user.
    /// </summary>
    public string? LastName { get; set; }

    /// <summary>
    /// Gets or sets the middle name of the user.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    public new string? PhoneNumber { get; set; }

    /// <summary>
    /// Gets or sets the domain-specific identifier (UserAccountId) for this user.
    /// This property creates a correlation between the Identity user and the domain UserAccount entity.
    /// </summary>
    public string? DomainId { get; set; }

    /// <summary>
    /// Gets or sets the identity provider that created this user account.
    /// </summary>
    public string? IdentityProvider { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the user must change their password on next login.
    /// </summary>
    public bool PasswordChangeRequired { get; set; }
}
