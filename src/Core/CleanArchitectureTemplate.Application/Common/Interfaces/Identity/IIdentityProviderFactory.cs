using CleanArchitectureTemplate.Domain.UserAccounts.Services;

namespace CleanArchitectureTemplate.Application.Common.Interfaces.Identity;

/// <summary>
/// Factory interface for creating identity service implementations.
/// Used to select between EF Core Identity and Microsoft Entra ID providers.
/// </summary>
public interface IIdentityProviderFactory
{
    /// <summary>
    /// Creates an instance of the ICurrentUserService based on the configured provider.
    /// </summary>
    /// <returns>An implementation of ICurrentUserService.</returns>
    ICurrentUserService CreateCurrentUserService();

    /// <summary>
    /// Creates an instance of the IIdentityService based on the configured provider.
    /// </summary>
    /// <returns>An implementation of IIdentityService.</returns>
    IIdentityService CreateIdentityService();

    /// <summary>
    /// Gets the name of the currently active identity provider.
    /// </summary>
    /// <returns>The name of the active identity provider.</returns>
    string GetActiveProviderName();
}