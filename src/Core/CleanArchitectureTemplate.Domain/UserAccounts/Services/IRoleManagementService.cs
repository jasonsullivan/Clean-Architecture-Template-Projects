using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Provides role management services for managing roles and their permissions.
/// </summary>
public interface IRoleManagementService
{
    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="name">The name of the role.</param>
    /// <param name="description">The description of the role.</param>
    /// <param name="isSystemDefined">A value indicating whether the role is system-defined.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the created role.</returns>
    Task<Result<ApplicationRole>> CreateRoleAsync(
        string name,
        string description,
        bool isSystemDefined = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by its identifier.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the role.</returns>
    Task<Result<ApplicationRole>> GetRoleByIdAsync(
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by its name.
    /// </summary>
    /// <param name="roleName">The name of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the role.</returns>
    Task<Result<ApplicationRole>> GetRoleByNameAsync(
        string roleName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of roles.</returns>
    Task<Result<IReadOnlyCollection<ApplicationRole>>> GetAllRolesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role's description.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="description">The new description.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> UpdateRoleDescriptionAsync(
        ApplicationRoleId roleId,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeleteRoleAsync(
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a permission to a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> AddPermissionToRoleAsync(
        ApplicationRoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a permission from a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> RemovePermissionFromRoleAsync(
        ApplicationRoleId roleId,
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions for a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of permissions.</returns>
    Task<Result<IReadOnlyCollection<Permission>>> GetPermissionsForRoleAsync(
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> AssignRoleToUserAsync(
        UserAccountId userAccountId,
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> RemoveRoleFromUserAsync(
        UserAccountId userAccountId,
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles for a user.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of roles.</returns>
    Task<Result<IReadOnlyCollection<ApplicationRole>>> GetRolesForUserAsync(
        UserAccountId userAccountId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users for a role.
    /// </summary>
    /// <param name="roleId">The identifier of the role.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of user accounts.</returns>
    Task<Result<IReadOnlyCollection<UserAccount>>> GetUsersForRoleAsync(
        ApplicationRoleId roleId,
        CancellationToken cancellationToken = default);
}
