using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a role is added to a user account.
/// </summary>
public sealed class UserAccountRoleAddedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the user account to which the role was added.
    /// </summary>
    public UserAccountId UserAccountId { get; }

    /// <summary>
    /// Gets the identifier of the role that was added to the user account.
    /// </summary>
    public ApplicationRoleId RoleId { get; }

    /// <summary>
    /// Gets the name of the role that was added to the user account.
    /// </summary>
    public string RoleName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountRoleAddedEvent"/> class.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account to which the role was added.</param>
    /// <param name="roleId">The identifier of the role that was added to the user account.</param>
    /// <param name="roleName">The name of the role that was added to the user account.</param>
    public UserAccountRoleAddedEvent(
        UserAccountId userAccountId,
        ApplicationRoleId roleId,
        string roleName)
    {
        UserAccountId = userAccountId;
        RoleId = roleId;
        RoleName = roleName;
    }
}
