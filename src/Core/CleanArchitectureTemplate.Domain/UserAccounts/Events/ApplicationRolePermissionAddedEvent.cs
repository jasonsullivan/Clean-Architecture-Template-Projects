using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a permission is added to an application role.
/// </summary>
public sealed class ApplicationRolePermissionAddedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the application role to which the permission was added.
    /// </summary>
    public ApplicationRoleId RoleId { get; }

    /// <summary>
    /// Gets the identifier of the permission that was added to the application role.
    /// </summary>
    public PermissionId PermissionId { get; }

    /// <summary>
    /// Gets the name of the permission that was added to the application role.
    /// </summary>
    public PermissionName PermissionName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRolePermissionAddedEvent"/> class.
    /// </summary>
    /// <param name="roleId">The identifier of the application role to which the permission was added.</param>
    /// <param name="permissionId">The identifier of the permission that was added to the application role.</param>
    /// <param name="permissionName">The name of the permission that was added to the application role.</param>
    public ApplicationRolePermissionAddedEvent(
        ApplicationRoleId roleId,
        PermissionId permissionId,
        PermissionName permissionName)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        PermissionName = permissionName;
    }
}
