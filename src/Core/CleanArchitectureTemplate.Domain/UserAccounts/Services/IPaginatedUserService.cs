using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Interface for paginated user operations.
/// </summary>
public interface IPaginatedUserService
{
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

    /// <summary>
    /// Gets all users without pagination.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing all users.</returns>
    /// <remarks>
    /// This method should be used with caution and only in scenarios where the number of users is known to be small.
    /// </remarks>
    Task<Result<IEnumerable<UserAccount>>> GetAllUsersAsync(CancellationToken cancellationToken = default);
}