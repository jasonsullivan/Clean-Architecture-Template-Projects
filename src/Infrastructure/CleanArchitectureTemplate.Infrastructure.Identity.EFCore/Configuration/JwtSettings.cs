namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Configuration;

/// <summary>
/// Settings for JWT token authentication.
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Gets or sets the issuer of the JWT token.
    /// </summary>
    public string Issuer { get; set; } = "CleanArchitectureTemplate";

    /// <summary>
    /// Gets or sets the audience of the JWT token.
    /// </summary>
    public string Audience { get; set; } = "CleanArchitectureTemplate.API";

    /// <summary>
    /// Gets or sets the secret key used for signing JWT tokens.
    /// </summary>
    public string SecretKey { get; set; } = "YourStrongSecretKeyHere_AtLeast32CharsLong!";

    /// <summary>
    /// Gets or sets the expiration time of JWT tokens in minutes.
    /// </summary>
    public int ExpirationTimeInMinutes { get; set; } = 60;
}
