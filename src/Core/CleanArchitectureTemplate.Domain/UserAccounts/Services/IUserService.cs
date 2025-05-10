using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Interface for user management services.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Gets the currently authenticated user account.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the user account if successful.</returns>
    Task<Result<UserAccount>> GetCurrentUserAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user account by its ID.
    /// </summary>
    /// <param name="userId">The user ID to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the user account if found.</returns>
    Task<Result<UserAccount>> GetUserByIdAsync(UserAccountId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user account by username.
    /// </summary>
    /// <param name="username">The username to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the user account if found.</returns>
    Task<Result<UserAccount>> GetUserByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a user account by email.
    /// </summary>
    /// <param name="email">The email to look up.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the user account if found.</returns>
    Task<Result<UserAccount>> GetUserByEmailAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new user account.
    /// </summary>
    /// <param name="userAccount">The user account to create.</param>
    /// <param name="password">The password for the new user (only used with EF Core Identity).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result with the created user ID if successful.</returns>
    Task<Result<UserAccountId>> CreateUserAsync(UserAccount userAccount, string password, CancellationToken cancellationToken = default);

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
    /// <param name="userId">The ID of the user to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> DeleteUserAsync(UserAccountId userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Changes a user's password.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="currentPassword">The current password.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> ChangePasswordAsync(UserAccountId userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of users.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The page size.</param>
    /// <param name="searchTerm">Optional search term to filter users.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the paginated list of users.</returns>
    Task<Result<IEnumerable<UserAccount>>> GetUsersAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total number of users, optionally filtered by a search term.
    /// </summary>
    /// <param name="searchTerm">Optional search term to filter users.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the total number of users.</returns>
    Task<Result<int>> GetUserCountAsync(string? searchTerm = null, CancellationToken cancellationToken = default);
}
