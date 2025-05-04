using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a role is removed from a user account.
/// </summary>
public sealed class UserAccountRoleRemovedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the user account from which the role was removed.
    /// </summary>
    public UserAccountId UserAccountId { get; }

    /// <summary>
    /// Gets the identifier of the role that was removed from the user account.
    /// </summary>
    public ApplicationRoleId RoleId { get; }

    /// <summary>
    /// Gets the name of the role that was removed from the user account.
    /// </summary>
    public string RoleName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountRoleRemovedEvent"/> class.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account from which the role was removed.</param>
    /// <param name="roleId">The identifier of the role that was removed from the user account.</param>
    /// <param name="roleName">The name of the role that was removed from the user account.</param>
    public UserAccountRoleRemovedEvent(
        UserAccountId userAccountId,
        ApplicationRoleId roleId,
        string roleName)
    {
        UserAccountId = userAccountId;
        RoleId = roleId;
        RoleName = roleName;
    }
}
