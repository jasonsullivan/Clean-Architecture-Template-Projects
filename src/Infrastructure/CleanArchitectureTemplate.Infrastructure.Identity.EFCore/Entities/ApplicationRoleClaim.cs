using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents a claim associated with an application role.
/// </summary>
public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleClaim"/> class.
    /// </summary>
    public ApplicationRoleClaim() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleClaim"/> class with a specified claim type and value.
    /// </summary>
    /// <param name="claimType">The type of the claim.</param>
    /// <param name="claimValue">The value of the claim.</param>
    public ApplicationRoleClaim(string claimType, string claimValue) : base()
    {
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}
