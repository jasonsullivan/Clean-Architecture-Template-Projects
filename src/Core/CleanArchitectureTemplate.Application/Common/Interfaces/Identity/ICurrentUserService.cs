using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for accessing the currently authenticated user's information.
/// This service is implemented by both identity providers (EF Core Identity and Microsoft Entra ID).
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the unique identifier of the current user.
    /// </summary>
    UserAccountId? UserId { get; }

    /// <summary>
    /// Gets the username of the current user.
    /// </summary>
    string? Username { get; }

    /// <summary>
    /// Gets the email address of the current user.
    /// </summary>
    Email? Email { get; }

    /// <summary>
    /// Checks if the current user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }

    /// <summary>
    /// Checks if the current user has the specified role.
    /// </summary>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the role, false otherwise.</returns>
    Task<bool> IsInRoleAsync(string role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the roles assigned to the current user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the collection of role names.</returns>
    Task<Result<IEnumerable<string>>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the current user has the specified permission.
    /// </summary>
    /// <param name="permission">The permission name to check.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the user has the permission, false otherwise.</returns>
    Task<bool> HasPermissionAsync(string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions assigned to the current user through their roles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result containing the collection of permission names.</returns>
    Task<Result<IEnumerable<string>>> GetPermissionsAsync(CancellationToken cancellationToken = default);
}
