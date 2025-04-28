using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents a role associated with an application user.
/// </summary>
public class ApplicationUserRole : IdentityUserRole<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserRole"/> class.
    /// </summary>
    public ApplicationUserRole() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserRole"/> class with a specified user ID and role ID.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="roleId">The role ID.</param>
    public ApplicationUserRole(string userId, string roleId) : base()
    {
        UserId = userId;
        RoleId = roleId;
    }
}
