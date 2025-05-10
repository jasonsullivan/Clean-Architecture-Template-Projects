using CleanArchitectureTemplate.Application.Common.Interfaces.Identity;
using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;
using CleanArchitectureTemplate.Shared.Primitives;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Security.Claims;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;

/// <summary>
/// Implementation of the IIdentityService interface using EF Core Identity.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EFCoreIdentityService"/> class.
/// </remarks>
/// <param name="userManager">The ASP.NET Core Identity user manager.</param>
/// <param name="roleManager">The ASP.NET Core Identity role manager.</param>
/// <param name="dbContext">The identity database context.</param>
/// <param name="mapper">The service for mapping between identity and domain entities.</param>
/// <param name="logger">The logger instance.</param>
public class EFCoreIdentityService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    ApplicationIdentityDbContext dbContext,
    IdentityMapper mapper,
    ILogger<EFCoreIdentityService> logger) : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationIdentityDbContext _dbContext = dbContext;
    private readonly IdentityMapper _mapper = mapper;
    private readonly ILogger<EFCoreIdentityService> _logger = logger;

    /// <inheritdoc/>
    public async Task<Result<UserAccount>> GetUserByIdAsync(UserAccountId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found", userId.Value);
                return Result<UserAccount>.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userId.Value} not found."));
            }

            var userAccount = _mapper.MapToUserAccount(user);
            if (userAccount == null)
            {
                _logger.LogError("Failed to map user with ID {UserId} to domain entity", userId.Value);
                return Result<UserAccount>.Failure(DomainError.Failure("Identity.MappingFailed", "Failed to map user to domain entity."));
            }

            return Result<UserAccount>.Success(userAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {UserId}", userId.Value);
            return Result<UserAccount>.Failure(DomainError.Problem("Identity.Error", $"Error retrieving user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UserAccount>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByNameAsync(username).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with username '{Username}' not found", username);
                return Result<UserAccount>.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with username '{username}' not found."));
            }

            var userAccount = _mapper.MapToUserAccount(user);
            if (userAccount == null)
            {
                _logger.LogError("Failed to map user with username '{Username}' to domain entity", username);
                return Result<UserAccount>.Failure(DomainError.Failure("Identity.MappingFailed", "Failed to map user to domain entity."));
            }

            return Result<UserAccount>.Success(userAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with username '{Username}'", username);
            return Result<UserAccount>.Failure(DomainError.Problem("Identity.Error", $"Error retrieving user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UserAccount>> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email.Value).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with email '{Email}' not found", email.Value);
                return Result<UserAccount>.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with email '{email.Value}' not found."));
            }

            var userAccount = _mapper.MapToUserAccount(user);
            if (userAccount == null)
            {
                _logger.LogError("Failed to map user with email '{Email}' to domain entity", email.Value);
                return Result<UserAccount>.Failure(DomainError.Failure("Identity.MappingFailed", "Failed to map user to domain entity."));
            }

            return Result<UserAccount>.Success(userAccount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with email '{Email}'", email.Value);
            return Result<UserAccount>.Failure(DomainError.Problem("Identity.Error", $"Error retrieving user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<UserAccountId>> CreateUserAsync(UserAccount userAccount, string? password = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // For EF Core Identity, a password is required
            if (string.IsNullOrEmpty(password))
            {
                return Result<UserAccountId>.Failure(DomainError.Failure("Identity.PasswordRequired", "Password is required for EF Core Identity."));
            }

            // Check if user with same domain ID already exists
            var existingUser = await _dbContext.Users
                .Where(u => string.Equals(u.DomainId, userAccount.Id.Value.ToString(), StringComparison.Ordinal))
                .FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);

            if (existingUser != null)
            {
                return Result<UserAccountId>.Failure(DomainError.Conflict("Identity.DuplicateDomainId", "A user with this domain ID already exists."));
            }

            // Check if user with same email already exists
            var existingEmail = await _userManager.FindByEmailAsync(userAccount.Email.Value).ConfigureAwait(false);
            if (existingEmail != null)
            {
                return Result<UserAccountId>.Failure(DomainError.Conflict("Identity.DuplicateEmail", "A user with this email already exists."));
            }

            // Check if user with same username already exists
            var existingUsername = await _userManager.FindByNameAsync(userAccount.Username.Value).ConfigureAwait(false);
            if (existingUsername != null)
            {
                return Result<UserAccountId>.Failure(DomainError.Conflict("Identity.DuplicateUsername", "A user with this username already exists."));
            }

            // Create new ApplicationUser from UserAccount
            var user = _mapper.MapToApplicationUser(userAccount);

            // Set identity provider
            user.IdentityProvider = "EFCoreIdentity";

            // Create the user with password
            var result = await _userManager.CreateAsync(user, password).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result<UserAccountId>.Failure([.. errors]);
            }

            // Get the generated UserAccountId from the mapping
            var userAccountResult = UserAccountId.Create(Guid.Parse(user.DomainId!));
            if (!userAccountResult.IsSuccess)
            {
                // This should not happen since we set the DomainId in the mapper
                _logger.LogError("Failed to create UserAccountId from DomainId {DomainId}", user.DomainId);
                await _userManager.DeleteAsync(user).ConfigureAwait(false); // Clean up the created user
                return Result<UserAccountId>.Failure(userAccountResult.Errors);
            }

            _logger.LogInformation("Created new user with ID {UserId}", userAccountResult.Value.Value);
            return Result<UserAccountId>.Success(userAccountResult.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user with ID {UserId}", userAccount.Id.Value);
            return Result<UserAccountId>.Failure(DomainError.Problem("Identity.Error", $"Error creating user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateUserAsync(UserAccount userAccount, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for update", userAccount.Id.Value);
                return Result.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userAccount.Id.Value} not found."));
            }

            // Check if email is being changed and if it's already in use
            if (string.Compare(user.Email, userAccount.Email.Value, StringComparison.Ordinal) != 0)
            {
                var existingEmail = await _userManager.FindByEmailAsync(userAccount.Email.Value).ConfigureAwait(false);
                if (existingEmail != null && string.Compare(existingEmail.Id, user.Id, StringComparison.Ordinal) != 0)
                {
                    return Result.Failure(DomainError.Conflict("Identity.DuplicateEmail", "A user with this email already exists."));
                }
            }

            // Check if username is being changed and if it's already in use
            if (string.Compare(user.UserName, userAccount.Username.Value, StringComparison.Ordinal) != 0)
            {
                var existingUsername = await _userManager.FindByNameAsync(userAccount.Username.Value).ConfigureAwait(false);
                if (existingUsername != null && string.Compare(existingUsername.Id, user.Id, StringComparison.Ordinal) != 0)
                {
                    return Result.Failure(DomainError.Conflict("Identity.DuplicateUsername", "A user with this username already exists."));
                }
            }

            // Map the domain entity to ApplicationUser, preserving the identity ID
            user = _mapper.MapToApplicationUser(userAccount, user);

            // Update the user
            var result = await _userManager.UpdateAsync(user).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            _logger.LogInformation("Updated user with ID {UserId}", userAccount.Id.Value);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {UserId}", userAccount.Id.Value);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error updating user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteUserAsync(UserAccountId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for deletion", userId.Value);
                return Result.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userId.Value} not found."));
            }

            var result = await _userManager.DeleteAsync(user).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            _logger.LogInformation("Deleted user with ID {UserId}", userId.Value);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {UserId}", userId.Value);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error deleting user: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> AddUserToRoleAsync(UserAccountId userId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for role assignment", userId.Value);
                return Result.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userId.Value} not found."));
            }

            // Check if the role exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
            if (!roleExists)
            {
                _logger.LogWarning("Role '{RoleName}' not found", roleName);
                return Result.Failure(DomainError.NotFound("Identity.RoleNotFound", $"Role '{roleName}' not found."));
            }

            // Check if user is already in role
            var isInRole = await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false);
            if (isInRole)
            {
                _logger.LogInformation("User with ID {UserId} is already in role '{RoleName}'", userId.Value, roleName);
                return Result.Success(); // Already in role, consider it a success
            }

            var result = await _userManager.AddToRoleAsync(user, roleName).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            _logger.LogInformation("Added user with ID {UserId} to role '{RoleName}'", userId.Value, roleName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding user with ID {UserId} to role '{RoleName}'", userId.Value, roleName);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error adding user to role: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> RemoveUserFromRoleAsync(UserAccountId userId, string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for role removal", userId.Value);
                return Result.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userId.Value} not found."));
            }

            // Check if the user is in the role
            var isInRole = await _userManager.IsInRoleAsync(user, roleName).ConfigureAwait(false);
            if (!isInRole)
            {
                _logger.LogWarning("User with ID {UserId} is not in role '{RoleName}'", userId.Value, roleName);
                return Result.Failure(DomainError.Failure("Identity.UserNotInRole", $"User is not in role '{roleName}'."));
            }

            var result = await _userManager.RemoveFromRoleAsync(user, roleName).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            _logger.LogInformation("Removed user with ID {UserId} from role '{RoleName}'", userId.Value, roleName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing user with ID {UserId} from role '{RoleName}'", userId.Value, roleName);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error removing user from role: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>>> GetUserRolesAsync(UserAccountId userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await FindUserByIdAsync(userId, cancellationToken).ConfigureAwait(false);
            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for getting roles", userId.Value);
                return Result<IEnumerable<string>>.Failure(DomainError.NotFound("Identity.UserNotFound", $"User with ID {userId.Value} not found."));
            }

            var roles = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            return Result<IEnumerable<string>>.Success(roles.AsEnumerable());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user with ID {UserId}", userId.Value);
            return Result<IEnumerable<string>>.Failure(DomainError.Problem("Identity.Error", $"Error getting user roles: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result<IEnumerable<string>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var roles = await _roleManager.Roles
                .Select(r => r.Name)
                .Where(n => n != null)
                .ToListAsync(cancellationToken).ConfigureAwait(false);

            return Result<IEnumerable<string>>.Success(roles.Cast<string>());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all roles");
            return Result<IEnumerable<string>>.Failure(DomainError.Problem("Identity.Error", $"Error getting roles: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> CreateRoleAsync(string roleName, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            // Check if the role already exists
            var roleExists = await _roleManager.RoleExistsAsync(roleName).ConfigureAwait(false);
            if (roleExists)
            {
                _logger.LogWarning("Role '{RoleName}' already exists", roleName);
                return Result.Failure(DomainError.Conflict("Identity.RoleAlreadyExists", $"Role '{roleName}' already exists."));
            }

            var role = new ApplicationRole(roleName);
            var result = await _roleManager.CreateAsync(role).ConfigureAwait(false);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            // Add permissions as claims to the role
            foreach (var permission in permissions)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission)).ConfigureAwait(false);
            }

            _logger.LogInformation("Created role '{RoleName}' with {PermissionCount} permissions", roleName, permissions.Count());
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role '{RoleName}'", roleName);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error creating role: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> UpdateRoleAsync(string roleName, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role == null)
            {
                _logger.LogWarning("Role '{RoleName}' not found for update", roleName);
                return Result.Failure(DomainError.NotFound("Identity.RoleNotFound", $"Role '{roleName}' not found."));
            }

            // Get existing permission claims
            var claims = await _roleManager.GetClaimsAsync(role).ConfigureAwait(false);
            var existingPermissions = claims
                .Where(c => string.Compare(c.Type, "Permission", StringComparison.Ordinal) == 0)
                .Select(c => c.Value)
                .ToList();

            // Remove permissions that are no longer needed
            foreach (var permission in existingPermissions)
            {
                if (!permissions.Contains(permission))
                {
                    await _roleManager.RemoveClaimAsync(role, new Claim("Permission", permission)).ConfigureAwait(false);
                }
            }

            // Add new permissions
            foreach (var permission in permissions)
            {
                if (!existingPermissions.Contains(permission))
                {
                    await _roleManager.AddClaimAsync(role, new Claim("Permission", permission)).ConfigureAwait(false);
                }
            }

            _logger.LogInformation("Updated role '{RoleName}' with {PermissionCount} permissions", roleName, permissions.Count());
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role '{RoleName}'", roleName);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error updating role: {ex.Message}"));
        }
    }

    /// <inheritdoc/>
    public async Task<Result> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await _roleManager.FindByNameAsync(roleName).ConfigureAwait(false);
            if (role == null)
            {
                _logger.LogWarning("Role '{RoleName}' not found for deletion", roleName);
                return Result.Failure(DomainError.NotFound("Identity.RoleNotFound", $"Role '{roleName}' not found."));
            }

            // Check if there are users in this role before deleting
            var usersInRole = await _userManager.GetUsersInRoleAsync(roleName).ConfigureAwait(false);
            if (usersInRole.Any())
            {
                _logger.LogWarning("Role '{RoleName}' is in use by {UserCount} users and cannot be deleted", roleName, usersInRole.Count);
                return Result.Failure(DomainError.Conflict("Identity.RoleInUse", $"Role '{roleName}' is in use and cannot be deleted."));
            }

            var result = await _roleManager.DeleteAsync(role).ConfigureAwait(false);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => DomainError.Failure(e.Code, e.Description));
                return Result.Failure([.. errors]);
            }

            _logger.LogInformation("Deleted role '{RoleName}'", roleName);
            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role '{RoleName}'", roleName);
            return Result.Failure(DomainError.Problem("Identity.Error", $"Error deleting role: {ex.Message}"));
        }
    }

    /// <summary>
    /// Finds a user by their domain ID or identity ID.
    /// </summary>
    /// <param name="userId">The user ID to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found user or null if not found.</returns>
    private async Task<ApplicationUser?> FindUserByIdAsync(UserAccountId userId, CancellationToken cancellationToken = default)
    {
        if (userId == null || userId.Value == Guid.Empty)
        {
            _logger.LogWarning("Invalid UserAccountId provided: {UserId}", userId?.Value);
            return null;
        }

        // Ensure the DomainId comparison is case-insensitive and handles potential mismatches
        var user = await _dbContext.Users
            .Where(u => string.Equals(u.DomainId, userId.Value.ToString(), StringComparison.Ordinal))
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if (user == null)
        {
            _logger.LogDebug("User not found by DomainId: {DomainId}. Attempting to find by IdentityId.", userId.Value);

            // If not found by DomainId, attempt to find by IdentityId
            user = await _userManager.FindByIdAsync(userId.Value.ToString()).ConfigureAwait(false);

            if (user == null)
            {
                _logger.LogWarning("User not found by IdentityId: {IdentityId}", userId.Value);
            }
        }

        return user;
    }
}
