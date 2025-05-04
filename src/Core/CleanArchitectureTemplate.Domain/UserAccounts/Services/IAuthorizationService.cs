using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Provides authorization services for checking user permissions.
/// </summary>
public interface IAuthorizationService
{
    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="permissionId">The identifier of the permission to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{Boolean}"/> indicating whether the user has the permission.</returns>
    Task<Result<bool>> HasPermissionAsync(
        UserAccountId userAccountId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific permission.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="permissionName">The name of the permission to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{Boolean}"/> indicating whether the user has the permission.</returns>
    Task<Result<bool>> HasPermissionAsync(
        UserAccountId userAccountId,
        PermissionName permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a user.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of permissions for the user.</returns>
    Task<Result<IReadOnlyCollection<PermissionName>>> GetPermissionsForUserAsync(
        UserAccountId userAccountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleId">The identifier of the role to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{Boolean}"/> indicating whether the user has the role.</returns>
    Task<Result<bool>> HasRoleAsync(
        UserAccountId userAccountId,
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user has a specific role.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleName">The name of the role to check.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{Boolean}"/> indicating whether the user has the role.</returns>
    Task<Result<bool>> HasRoleAsync(
        UserAccountId userAccountId,
        string roleName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a user.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of roles for the user.</returns>
    Task<Result<IReadOnlyCollection<string>>> GetRolesForUserAsync(
        UserAccountId userAccountId,
        CancellationToken cancellationToken = default);
}
