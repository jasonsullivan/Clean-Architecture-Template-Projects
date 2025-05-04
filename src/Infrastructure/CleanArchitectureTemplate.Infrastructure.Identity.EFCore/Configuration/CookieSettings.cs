namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Configuration;

/// <summary>
/// Settings for cookie authentication.
/// </summary>
public class CookieSettings
{
    /// <summary>
    /// Gets or sets the expiration time of cookies in minutes.
    /// </summary>
    public int ExpirationTimeInMinutes { get; set; } = 30;

    /// <summary>
    /// Gets or sets a value indicating whether sliding expiration is enabled.
    /// </summary>
    public bool SlidingExpiration { get; set; } = true;

    /// <summary>
    /// Gets or sets the login path.
    /// </summary>
    public string LoginPath { get; set; } = "/Account/Login";

    /// <summary>
    /// Gets or sets the logout path.
    /// </summary>
    public string LogoutPath { get; set; } = "/Account/Logout";

    /// <summary>
    /// Gets or sets the access denied path.
    /// </summary>
    public string AccessDeniedPath { get; set; } = "/Account/AccessDenied";
}
