using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a permission's description is changed.
/// </summary>
public sealed class PermissionDescriptionChangedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the permission whose description was changed.
    /// </summary>
    public PermissionId PermissionId { get; }

    /// <summary>
    /// Gets the old description of the permission.
    /// </summary>
    public string OldDescription { get; }

    /// <summary>
    /// Gets the new description of the permission.
    /// </summary>
    public string NewDescription { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionDescriptionChangedEvent"/> class.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission whose description was changed.</param>
    /// <param name="oldDescription">The old description of the permission.</param>
    /// <param name="newDescription">The new description of the permission.</param>
    public PermissionDescriptionChangedEvent(
        PermissionId permissionId,
        string oldDescription,
        string newDescription)
    {
        PermissionId = permissionId;
        OldDescription = oldDescription;
        NewDescription = newDescription;
    }
}
