using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Services;

/// <summary>
/// Interface for password management operations.
/// </summary>
public interface IPasswordManagementService
{
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
    /// Resets a user's password (admin function).
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="requirePasswordChange">Whether to require the user to change their password on next login.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> ResetPasswordAsync(UserAccountId userId, string newPassword, bool requirePasswordChange = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a password reset token for a user.
    /// </summary>
    /// <param name="email">The email of the user.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result containing the reset token if successful.</returns>
    Task<Result<string>> GeneratePasswordResetTokenAsync(Email email, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validates a password reset token for a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="token">The reset token to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the validation.</returns>
    Task<Result> ValidatePasswordResetTokenAsync(UserAccountId userId, string token, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="token">The reset token.</param>
    /// <param name="newPassword">The new password.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    Task<Result> ResetPasswordWithTokenAsync(UserAccountId userId, string token, string newPassword, CancellationToken cancellationToken = default);
}