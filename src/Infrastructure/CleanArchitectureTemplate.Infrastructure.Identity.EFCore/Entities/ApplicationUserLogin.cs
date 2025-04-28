using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

/// <summary>
/// Represents a login associated with an application user.
/// </summary>
public class ApplicationUserLogin : IdentityUserLogin<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserLogin"/> class.
    /// </summary>
    public ApplicationUserLogin() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationUserLogin"/> class with a specified login provider and provider key.
    /// </summary>
    /// <param name="loginProvider">The login provider.</param>
    /// <param name="providerKey">The provider key.</param>
    public ApplicationUserLogin(string loginProvider, string providerKey) : base()
    {
        LoginProvider = loginProvider;
        ProviderKey = providerKey;
    }
}
