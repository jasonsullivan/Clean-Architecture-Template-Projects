using CleanArchitectureTemplate.Domain.UserAccounts.Enums;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a permission's type is changed.
/// </summary>
public sealed class PermissionTypeChangedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the permission whose type was changed.
    /// </summary>
    public PermissionId PermissionId { get; }

    /// <summary>
    /// Gets the old type of the permission.
    /// </summary>
    public PermissionType OldType { get; }

    /// <summary>
    /// Gets the new type of the permission.
    /// </summary>
    public PermissionType NewType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionTypeChangedEvent"/> class.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission whose type was changed.</param>
    /// <param name="oldType">The old type of the permission.</param>
    /// <param name="newType">The new type of the permission.</param>
    public PermissionTypeChangedEvent(
        PermissionId permissionId,
        PermissionType oldType,
        PermissionType newType)
    {
        PermissionId = permissionId;
        OldType = oldType;
        NewType = newType;
    }
}
