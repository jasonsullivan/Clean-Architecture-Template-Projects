using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.Enums;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Provides permission management services for managing permissions.
/// </summary>
public interface IPermissionManagementService
{
    /// <summary>
    /// Creates a new permission.
    /// </summary>
    /// <param name="name">The name of the permission.</param>
    /// <param name="description">The description of the permission.</param>
    /// <param name="type">The type of the permission.</param>
    /// <param name="isSystemDefined">A value indicating whether the permission is system-defined.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the created permission.</returns>
    Task<Result<Permission>> CreatePermissionAsync(
        PermissionName name,
        string description,
        PermissionType type,
        bool isSystemDefined = false,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a permission by its identifier.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the permission.</returns>
    Task<Result<Permission>> GetPermissionByIdAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a permission by its name.
    /// </summary>
    /// <param name="permissionName">The name of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the permission.</returns>
    Task<Result<Permission>> GetPermissionByNameAsync(
        PermissionName permissionName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all permissions.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of permissions.</returns>
    Task<Result<IReadOnlyCollection<Permission>>> GetAllPermissionsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a permission's description.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="description">The new description.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> UpdatePermissionDescriptionAsync(
        PermissionId permissionId,
        string description,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a permission's type.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="type">The new type.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> UpdatePermissionTypeAsync(
        PermissionId permissionId,
        PermissionType type,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a permission.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    Task<Result> DeletePermissionAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a standard set of CRUD permissions for a resource.
    /// </summary>
    /// <param name="resource">The resource name (e.g., "Users", "Projects").</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of created permissions.</returns>
    Task<Result<IReadOnlyCollection<Permission>>> CreateStandardPermissionsAsync(
        string resource,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles that have a specific permission.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of roles.</returns>
    Task<Result<IReadOnlyCollection<ApplicationRole>>> GetRolesForPermissionAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all users that have a specific permission.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A <see cref="Result{T}"/> containing the collection of user accounts.</returns>
    Task<Result<IReadOnlyCollection<UserAccount>>> GetUsersForPermissionAsync(
        PermissionId permissionId,
        CancellationToken cancellationToken = default);
}
