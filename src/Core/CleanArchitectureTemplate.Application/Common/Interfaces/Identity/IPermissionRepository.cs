using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Application.Common.Interfaces.Identity;

/// <summary>
/// Repository interface for managing permissions.
/// </summary>
public interface IPermissionRepository
{
    /// <summary>
    /// Gets a permission by its identifier.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission if found, otherwise null.</returns>
    Task<Permission?> GetPermissionByIdAsync(PermissionId permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a permission by its name.
    /// </summary>
    /// <param name="permissionName">The permission name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The permission if found, otherwise null.</returns>
    Task<Permission?> GetPermissionByNameAsync(PermissionName permissionName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of permissions.</returns>
    Task<IReadOnlyCollection<Permission>> GetAllPermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="permission">The permission to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation containing the created permission's ID.</returns>
    Task<Result<PermissionId>> CreatePermissionAsync(Permission permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a permission.
    /// </summary>
    /// <param name="permission">The permission to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> UpdatePermissionAsync(Permission permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    /// <param name="permissionId">The permission identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> DeletePermissionAsync(PermissionId permissionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a specific role.
    /// </summary>
    /// <param name="role">The role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of permissions assigned to the role.</returns>
    Task<IReadOnlyCollection<Permission>> GetPermissionsForRoleAsync(DomainRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a specific user.
    /// </summary>
    /// <param name="userAccountId">The user account identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of permissions the user has access to.</returns>
    Task<IReadOnlyCollection<Permission>> GetPermissionsForUserAsync(UserAccountId userAccountId, CancellationToken cancellationToken = default);
}