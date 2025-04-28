using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents a claim associated with an application user.
/// </summary>
public class ApplicationUserClaim : IdentityUserClaim<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserClaim"/> class.
    /// </summary>
    public ApplicationUserClaim() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserClaim"/> class with a specified claim type and value.
    /// </summary>
    /// <param name="claimType">The type of the claim.</param>
    /// <param name="claimValue">The value of the claim.</param>
    public ApplicationUserClaim(string claimType, string claimValue) : base()
    {
        ClaimType = claimType;
        ClaimValue = claimValue;
    }
}
