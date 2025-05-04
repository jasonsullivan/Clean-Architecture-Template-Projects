using CleanArchitectureTemplate.Domain.UserAccounts.Enums;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when a permission is created.
/// </summary>
public sealed class PermissionCreatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the permission that was created.
    /// </summary>
    public PermissionId PermissionId { get; }

    /// <summary>
    /// Gets the name of the permission that was created.
    /// </summary>
    public PermissionName Name { get; }

    /// <summary>
    /// Gets the description of the permission that was created.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the type of the permission that was created.
    /// </summary>
    public PermissionType Type { get; }

    /// <summary>
    /// Gets a value indicating whether the permission is system-defined.
    /// </summary>
    public bool IsSystemDefined { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionCreatedEvent"/> class.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission that was created.</param>
    /// <param name="name">The name of the permission that was created.</param>
    /// <param name="description">The description of the permission that was created.</param>
    /// <param name="type">The type of the permission that was created.</param>
    /// <param name="isSystemDefined">A value indicating whether the permission is system-defined.</param>
    public PermissionCreatedEvent(
        PermissionId permissionId,
        PermissionName name,
        string description,
        PermissionType type,
        bool isSystemDefined)
    {
        PermissionId = permissionId;
        Name = name;
        Description = description;
        Type = type;
        IsSystemDefined = isSystemDefined;
    }
}
