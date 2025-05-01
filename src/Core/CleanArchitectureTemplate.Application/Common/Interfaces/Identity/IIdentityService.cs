using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Application.Common.Interfaces.Identity;

/// <summary>
/// Interface for identity management operations.
/// This service is implemented by both identity providers (EF Core Identity and Microsoft Entra ID).
/// </summary>
public interface IIdentityService
{
    /// <summary>
    /// Gets a user account by its unique identifier.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing user account if found, or an error if not found.</returns>
    Task<Result<UserAccount>> GetUserByIdAsync(UserAccountId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user account by username.
    /// </summary>
    /// <param name="username">The username to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing user account if found, or an error if not found.</returns>
    Task<Result<UserAccount>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user account by email.
    /// </summary>
    /// <param name="email">The email to search for.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing user account if found, or an error if not found.</returns>
    Task<Result<UserAccount>> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="userAccount">The user account to create.</param>
    /// <param name="password">The password for the new user (only used with EF Core Identity).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation containing the created user ID or error information.</returns>
    Task<Result<UserAccountId>> CreateUserAsync(UserAccount userAccount, string? password = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing user account.
    /// </summary>
    /// <param name="userAccount">The user account to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> UpdateUserAsync(UserAccount userAccount, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a user account.
    /// </summary>
    /// <param name="userId">The unique identifier of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> DeleteUserAsync(UserAccountId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleName">The name of the role to assign.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> AddUserToRoleAsync(UserAccountId userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a role from a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="roleName">The name of the role to remove.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> RemoveUserFromRoleAsync(UserAccountId userId, string roleName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all roles assigned to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing collection of role names.</returns>
    Task<Result<IEnumerable<string>>> GetUserRolesAsync(UserAccountId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available roles in the system.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing collection of role names.</returns>
    Task<Result<IEnumerable<string>>> GetRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new role.
    /// </summary>
    /// <param name="roleName">The name of the role to create.</param>
    /// <param name="permissions">The permissions to assign to the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> CreateRoleAsync(string roleName, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    /// <param name="roleName">The name of the role to update.</param>
    /// <param name="permissions">The permissions to assign to the role.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> UpdateRoleAsync(string roleName, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a role.
    /// </summary>
    /// <param name="roleName">The name of the role to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> DeleteRoleAsync(string roleName, CancellationToken cancellationToken = default);
}