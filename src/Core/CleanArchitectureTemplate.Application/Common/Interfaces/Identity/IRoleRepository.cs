using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Application.Common.Interfaces.Identity;

/// <summary>
/// Repository interface for managing application roles.
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Gets a role by its identifier.
    /// </summary>
    /// <param name="roleId">The role identifier.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The domain role if found, otherwise null.</returns>
    Task<DomainRole?> GetRoleByIdAsync(DomainRoleId roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a role by its name.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The domain role if found, otherwise null.</returns>
    Task<DomainRole?> GetRoleByNameAsync(string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of domain roles.</returns>
    Task<IReadOnlyCollection<DomainRole>> GetAllRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role.
    /// </summary>
    /// <param name="role">The role to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> UpdateRoleAsync(DomainRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a role's permissions.
    /// </summary>
    /// <param name="role">The role with updated permissions.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> UpdateRolePermissionsAsync(DomainRole role, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets users assigned to a specific role.
    /// </summary>
    /// <param name="roleName">The role name.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A collection of user accounts in the role.</returns>
    Task<IReadOnlyCollection<UserAccount>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken = default);
}