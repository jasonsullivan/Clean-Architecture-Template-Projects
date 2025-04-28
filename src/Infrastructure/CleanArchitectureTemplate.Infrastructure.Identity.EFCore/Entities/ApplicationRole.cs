using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents an application role with extended constructors.
/// </summary>
public class ApplicationRole : IdentityRole
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRole"/> class.
    /// </summary>
    public ApplicationRole() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRole"/> class with a specified role name.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    public ApplicationRole(string roleName) : base(roleName)
    {
    }
}
