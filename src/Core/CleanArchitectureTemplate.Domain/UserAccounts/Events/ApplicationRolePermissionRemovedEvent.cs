using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a permission is removed from an application role.
/// </summary>
public sealed class ApplicationRolePermissionRemovedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the application role from which the permission was removed.
    /// </summary>
    public DomainRoleId RoleId { get; }

    /// <summary>
    /// Gets the identifier of the permission that was removed from the application role.
    /// </summary>
    public PermissionId PermissionId { get; }

    /// <summary>
    /// Gets the name of the permission that was removed from the application role.
    /// </summary>
    public PermissionName PermissionName { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRolePermissionRemovedEvent"/> class.
    /// </summary>
    /// <param name="roleId">The identifier of the application role from which the permission was removed.</param>
    /// <param name="permissionId">The identifier of the permission that was removed from the application role.</param>
    /// <param name="permissionName">The name of the permission that was removed from the application role.</param>
    public ApplicationRolePermissionRemovedEvent(
        DomainRoleId roleId,
        PermissionId permissionId,
        PermissionName permissionName)
    {
        RoleId = roleId;
        PermissionId = permissionId;
        PermissionName = permissionName;
    }
}
