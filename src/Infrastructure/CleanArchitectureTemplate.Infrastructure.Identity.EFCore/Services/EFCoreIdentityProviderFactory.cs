using CleanArchitectureTemplate.Application.Common.Interfaces.Identity;
using CleanArchitectureTemplate.Domain.UserAccounts.Services;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;

/// <summary>
/// Implementation of IIdentityProviderFactory for EF Core Identity.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EFCoreIdentityProviderFactory"/> class.
/// </remarks>
/// <param name="identityService">The EF Core Identity service implementation.</param>
/// <param name="currentUserService">The EF Core current user service implementation.</param>
/// <param name="options">The options for EF Core Identity.</param>
/// <param name="logger">The logger instance.</param>
public class EFCoreIdentityProviderFactory(
    EFCoreIdentityService identityService,
    EFCoreCurrentUserService currentUserService,
    IOptions<EFCoreIdentityOptions> options,
    ILogger<EFCoreIdentityProviderFactory> logger) : IIdentityProviderFactory
{
    private readonly EFCoreIdentityService _identityService = identityService;
    private readonly EFCoreCurrentUserService _currentUserService = currentUserService;
    private readonly EFCoreIdentityOptions _options = options.Value;
    private readonly ILogger<EFCoreIdentityProviderFactory> _logger = logger;

    /// <inheritdoc/>
    public ICurrentUserService CreateCurrentUserService()
    {
        _logger.LogDebug("Creating EF Core Current User Service");
        return _currentUserService;
    }

    /// <inheritdoc/>
    public IIdentityService CreateIdentityService()
    {
        _logger.LogDebug("Creating EF Core Identity Service");
        return _identityService;
    }

    /// <inheritdoc/>
    public string GetActiveProviderName()
    {
        return "EFCoreIdentity";
    }
}

/// <summary>
/// Options for configuring EF Core Identity.
/// </summary>
public class EFCoreIdentityOptions
{
    /// <summary>
    /// Gets or sets the connection string for the identity database.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database provider to use for the identity database.
    /// </summary>
    public string DatabaseProvider { get; set; } = "SqlServer";

    /// <summary>
    /// Gets or sets a value indicating whether to enable automatic migrations.
    /// </summary>
    public bool EnableAutoMigrations { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether to seed default data.
    /// </summary>
    public bool SeedDefaultData { get; set; } = true;

    /// <summary>
    /// Gets or sets the password options.
    /// </summary>
    public PasswordOptions PasswordOptions { get; set; } = new PasswordOptions();

    /// <summary>
    /// Gets or sets the user options.
    /// </summary>
    public UserOptions UserOptions { get; set; } = new UserOptions();

    /// <summary>
    /// Gets or sets the lockout options.
    /// </summary>
    public LockoutOptions LockoutOptions { get; set; } = new LockoutOptions();

    /// <summary>
    /// Gets or sets a value indicating whether to use domain ID claims.
    /// </summary>
    public bool UseDomainIdClaims { get; set; } = true;
}

/// <summary>
/// Options for configuring password requirements.
/// </summary>
public class PasswordOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether non-alphanumeric characters are required in passwords.
    /// </summary>
    public bool RequireNonAlphanumeric { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether uppercase letters are required in passwords.
    /// </summary>
    public bool RequireUppercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether lowercase letters are required in passwords.
    /// </summary>
    public bool RequireLowercase { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether digits are required in passwords.
    /// </summary>
    public bool RequireDigit { get; set; } = true;

    /// <summary>
    /// Gets or sets the minimum length required for passwords.
    /// </summary>
    public int RequiredLength { get; set; } = 8;

    /// <summary>
    /// Gets or sets the minimum number of unique characters required in passwords.
    /// </summary>
    public int RequiredUniqueChars { get; set; } = 1;
}

/// <summary>
/// Options for configuring user requirements.
/// </summary>
public class UserOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether email confirmation is required.
    /// </summary>
    public bool RequireEmailConfirmation { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether unique emails are required.
    /// </summary>
    public bool RequireUniqueEmail { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether phone number confirmation is required.
    /// </summary>
    public bool RequirePhoneNumberConfirmation { get; set; } = false;
}

/// <summary>
/// Options for configuring lockout behavior.
/// </summary>
public class LockoutOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether lockout is enabled.
    /// </summary>
    public bool EnableLockout { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of failed access attempts before lockout.
    /// </summary>
    public int MaxFailedAccessAttempts { get; set; } = 5;

    /// <summary>
    /// Gets or sets the lockout time span in minutes.
    /// </summary>
    public int LockoutTimeSpanInMinutes { get; set; } = 15;
}
