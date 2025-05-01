using CleanArchitectureTemplate.Application.Common.Interfaces.Identity;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;
using CleanArchitectureTemplate.Shared.Primitives;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;

/// <summary>
/// Implementation of the ICurrentUserService interface for EF Core Identity provider.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EFCoreCurrentUserService"/> class.
/// </remarks>
/// <param name="httpContextAccessor">The HTTP context accessor for accessing the current user claims.</param>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
/// <param name="roleManager">The ASP.NET Core Identity role manager.</param>
/// <param name="dbContext">The identity database context.</param>
/// <param name="logger">The logger instance.</param>
public class EFCoreCurrentUserService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationIdentityDbContext dbContext,
    ILogger<EFCoreCurrentUserService> logger) : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationIdentityDbContext _dbContext = dbContext;
    private readonly ILogger<EFCoreCurrentUserService> _logger = logger;

    // Cache the current user to avoid repeated database lookups
    private ApplicationUser? _currentUser;
    private bool _currentUserLoaded;

    /// <inheritdoc/>
    public UserAccountId? UserId
    {
        get
        {
            if (!IsAuthenticated)
            {
                return null;
            }

            // First check if we have a domain ID claim
            var domainIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("DomainId");
            if (!string.IsNullOrEmpty(domainIdClaim) && Guid.TryParse(domainIdClaim, out var domainGuid))
            {
                var userIdResult = UserAccountId.Create(domainGuid);
                if (userIdResult.IsSuccess)
                {
                    return userIdResult.Value;
                }
            }

            // If no domain ID claim or parsing failed, try to get the current user
            var currentUser = GetCurrentUserAsync().GetAwaiter().GetResult();
            if (currentUser != null && !string.IsNullOrEmpty(currentUser.DomainId) &&
                Guid.TryParse(currentUser.DomainId, out var userDomainGuid))
            {
                var userIdResult = UserAccountId.Create(userDomainGuid);
                if (userIdResult.IsSuccess)
                {
                    return userIdResult.Value;
                }
            }

            // Last resort: try to parse the identity ID as a GUID
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out var idGuid))
            {
                var userIdResult = UserAccountId.Create(idGuid);
                if (userIdResult.IsSuccess)
                {
                    return userIdResult.Value;
                }
            }

            _logger.LogWarning("Failed to determine UserAccountId for authenticated user");
            return null;
        }
    }

    /// <inheritdoc/>
    public string? Username => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name);

    /// <inheritdoc/>
    public Email? Email
    {
        get
        {
            var emailClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(emailClaim))
            {
                return null;
            }

            var emailResult = Email.Create(emailClaim);
            return emailResult.IsSuccess ? emailResult.Value : null;
        }
    }

    /// <inheritdoc/>
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    /// <inheritdoc/>
    public async Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated)
        {
            return false;
        }

        // Check if the role claim exists directly in the claims principal
        if (_httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false)
        {
            return true;
        }

        // If not found in claims, check through the user manager
        var user = await GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
        if (user == null)
        {
            return false;
        }

        return await _userManager.IsInRoleAsync(user, role).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated)
        {
            return Result<IEnumerable<string>>.Failure(DomainError.Failure("Identity.NotAuthenticated", "User is not authenticated."));
        }

        var user = await GetCurrentUserAsync(cancellationToken).ConfigureAwait(false);
        if (user == null)
        {
            return Result<IEnumerable<string>>.Failure(DomainError.Failure("Identity.UserNotFound", "User not found."));
        }

        var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        return Result<IEnumerable<string>>.Success(roles.AsEnumerable());
    }

    /// <inheritdoc/>
    public async Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated)
        {
            return false;
        }

        // Check if the permission claim exists directly in the claims principal
        var permissionClaim = _httpContextAccessor.HttpContext?.User?.FindAll("Permission")
            .Any(c => string.Equals(c.Value, permission, StringComparison.Ordinal));

        if (permissionClaim == true)
        {
            return true;
        }

        // If not found in claims, check through the roles
        var roles = await GetRolesAsync(cancellationToken).ConfigureAwait(false);
        if (!roles.IsSuccess)
        {
            return false;
        }

        // Check if the user has the permission through any of their roles
        foreach (var roleName in roles.Value)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role == null)
            {
                continue;
            }

            var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
            if (claims.Any(c => string.Equals(c.Type, "Permission", StringComparison.Ordinal) && string.Equals(c.Value, permission, StringComparison.Ordinal)))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>>> GetPermissionsAsync(CancellationToken cancellationToken = default)
    {
        if (!IsAuthenticated)
        {
            return Result<IEnumerable<string>>.Failure(DomainError.Failure("Identity.NotAuthenticated", "User is not authenticated."));
        }

        // Get the user's roles
        var roles = await GetRolesAsync(cancellationToken).ConfigureAwait(false);
        if (!roles.IsSuccess)
        {
            return Result<IEnumerable<string>>.Failure(roles.Errors);
        }

        // Collect all permissions from the user's roles
        var permissions = new HashSet<string>();

        // First add any permission claims directly in the claims principal
        var directPermissions = _httpContextAccessor.HttpContext?.User?.FindAll("Permission")
            .Select(c => c.Value);

        if (directPermissions != null)
        {
            foreach (var permission in directPermissions)
            {
                permissions.Add(permission);
            }
        }

        // Then add permissions from roles
        foreach (var roleName in roles.Value)
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role == null)
            {
                continue;
            }

            var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
            foreach (var claim in claims.Where(c => string.Equals(c.Type, "Permission", StringComparison.Ordinal)))
            {
                permissions.Add(claim.Value);
            }
        }

        return Result<IEnumerable<string>>.Success(permissions.AsEnumerable());
    }

    /// <summary>
    /// Gets the current user from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The current user or null if not found.</returns>
    private async Task<ApplicationUser?> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        // Return cached user if already loaded
        if (_currentUserLoaded)
        {
            return _currentUser;
        }

        var userId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
        {
            _currentUserLoaded = true;
            return null;
        }

        try
        {
            // First try to find by identity ID
            _currentUser = await _userManager.FindByIdAsync(userId).ConfigureAwait(false);

            // If not found and we have a domain ID claim, try to find by domain ID
            if (_currentUser == null)
            {
                var domainIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirstValue("DomainId");
                if (!string.IsNullOrEmpty(domainIdClaim))
                {
                    _currentUser = await _dbContext.Users
                        .FirstOrDefaultAsync(u => string.Equals(u.DomainId, domainIdClaim, StringComparison.Ordinal), cancellationToken).ConfigureAwait(false);
                }
            }

            _currentUserLoaded = true;
            return _currentUser;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user with ID {UserId}", userId);
            return null;
        }
    }
}
