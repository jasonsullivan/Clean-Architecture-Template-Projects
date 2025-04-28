using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents a token associated with an application user.
/// </summary>
public class ApplicationUserToken : IdentityUserToken<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserToken"/> class.
    /// </summary>
    public ApplicationUserToken() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserToken"/> class with a specified login provider and name.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="name">The name of the token.</param>
    public ApplicationUserToken(string loginProvider, string name) : base()
    {
        LoginProvider = loginProvider;
        Name = name;
    }
}
