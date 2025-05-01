using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;

/// <summary>
/// Mapper for converting between identity entities and domain entities.
/// Handles the correlation between ApplicationUser string IDs and UserAccount Guid IDs.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="IdentityMapper"/> class.
/// </remarks>
/// <param name="logger">Optional logger instance.</param>
public class IdentityMapper(ILogger<IdentityMapper>? logger = null)
{
    private static readonly string UserAccountIdClaimType = "UserAccountId";
    private readonly ILogger<IdentityMapper>? _logger = logger;

    /// <summary>
    /// Maps an ApplicationUser to a UserAccount domain entity.
    /// </summary>
    /// <param name="user">The ApplicationUser to map.</param>
    /// <returns>A UserAccount domain entity or null if mapping fails.</returns>
    public UserAccount? MapToUserAccount(ApplicationUser user)
    {
        if (user == null)
        {
            _logger?.LogWarning("Cannot map null ApplicationUser to UserAccount");
            return null;
        }

        try
        {
            // If the user has a DomainId property, use it
            if (!string.IsNullOrEmpty(user.DomainId) && Guid.TryParse(user.DomainId, out Guid userAccountGuid))
            {
                _logger?.LogDebug("Using DomainId {DomainId} for user {UserId}", user.DomainId, user.Id);
            }
            // Otherwise try to use the Identity ID if it's a valid GUID
            else if (Guid.TryParse(user.Id, out userAccountGuid))
            {
                _logger?.LogDebug("Using Identity ID {IdentityId} as GUID for user", user.Id);
            }
            // If we can't get a valid GUID, we can't map to a UserAccount
            else
            {
                _logger?.LogWarning("Could not determine a valid GUID for user {UserId}", user.Id);
                return null;
            }

            var userIdResult = UserAccountId.Create(userAccountGuid);
            if (!userIdResult.IsSuccess)
            {
                _logger?.LogWarning("Failed to create UserAccountId: {Errors}", string.Join(", ", userIdResult.Errors.Select(e => e.Description)));
                return null;
            }

            // Create the rest of the value objects with proper error handling
            var usernameResult = UserName.Create(user.UserName ?? string.Empty);
            if (!usernameResult.IsSuccess)
            {
                _logger?.LogWarning("Failed to create UserName: {Errors}", string.Join(", ", usernameResult.Errors.Select(e => e.Description)));
                return null;
            }

            var emailResult = Email.Create(user.Email ?? string.Empty);
            if (!emailResult.IsSuccess)
            {
                _logger?.LogWarning("Failed to create Email: {Errors}", string.Join(", ", emailResult.Errors.Select(e => e.Description)));
                return null;
            }

            // Determine user status from identity properties
            var status = user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow
                ? UserStatus.Inactive
                : user.EmailConfirmed
                    ? UserStatus.Active
                    : UserStatus.PendingActivation;

            // Create person name value object
            var personNameResult = PersonName.Create(
                user.FirstName ?? string.Empty,
                user.LastName ?? string.Empty,
                user.MiddleName);

            if (!personNameResult.IsSuccess)
            {
                _logger?.LogWarning("Failed to create PersonName: {Errors}", string.Join(", ", personNameResult.Errors.Select(e => e.Description)));
                return null;
            }

            // Create phone number value object if available
            PhoneNumber? phoneNumber = null;
            if (!string.IsNullOrWhiteSpace(user.PhoneNumber))
            {
                var phoneNumberResult = PhoneNumber.Create(user.PhoneNumber);
                if (phoneNumberResult.IsSuccess)
                {
                    phoneNumber = phoneNumberResult.Value;
                }
                else
                {
                    _logger?.LogWarning("Failed to create PhoneNumber, ignoring: {Errors}",
                        string.Join(", ", phoneNumberResult.Errors.Select(e => e.Description)));
                }
            }

            // Create the user account domain entity
            var userAccount = UserAccount.Create(
                userIdResult.Value,
                usernameResult.Value,
                emailResult.Value,
                status,
                personNameResult.Value,
                phoneNumber);

            _logger?.LogDebug("Successfully mapped ApplicationUser {UserId} to UserAccount {UserAccountId}",
                user.Id, userIdResult.Value.Value);

            return userAccount;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error mapping ApplicationUser {UserId} to UserAccount", user.Id);
            return null;
        }
    }

    /// <summary>
    /// Maps a UserAccount domain entity to an ApplicationUser.
    /// </summary>
    /// <param name="userAccount">The UserAccount domain entity to map.</param>
    /// <param name="existingUser">Optional existing ApplicationUser to update.</param>
    /// <returns>An ApplicationUser entity.</returns>
    public ApplicationUser MapToApplicationUser(UserAccount userAccount, ApplicationUser? existingUser = null)
    {
        if (userAccount == null)
        {
            _logger?.LogWarning("Cannot map null UserAccount to ApplicationUser");
            throw new ArgumentNullException(nameof(userAccount));
        }

        try
        {
            // Create a new user or use the existing one
            var user = existingUser ?? new ApplicationUser(userAccount.Username.Value);

            // Set the properties
            user.UserName = userAccount.Username.Value;
            user.Email = userAccount.Email.Value;
            user.NormalizedEmail = userAccount.Email.Value.ToUpperInvariant();
            user.NormalizedUserName = userAccount.Username.Value.ToUpperInvariant();
            user.FirstName = userAccount.PersonName.FirstName;
            user.LastName = userAccount.PersonName.LastName;
            user.MiddleName = userAccount.PersonName.MiddleName;
            user.PhoneNumber = userAccount.PhoneNumber?.Value;
            user.EmailConfirmed = userAccount.Status == UserStatus.Active;
            user.LockoutEnabled = true;

            // Store the domain ID for correlation
            user.DomainId = userAccount.Id.Value.ToString();

            // Set lockout end date if the user is suspended
            if (userAccount.Status == UserStatus.Inactive)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100); // Far future date to represent suspension
            }
            else
            {
                user.LockoutEnd = null;
            }

            _logger?.LogDebug("Successfully mapped UserAccount {UserAccountId} to ApplicationUser {UserId}",
                userAccount.Id.Value, user.Id);

            return user;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error mapping UserAccount {UserAccountId} to ApplicationUser", userAccount.Id.Value);
            throw; // Rethrow to allow caller to handle
        }
    }
}
