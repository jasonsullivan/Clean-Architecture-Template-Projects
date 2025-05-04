using CleanArchitectureTemplate.Domain.UserAccounts.Enums;
using CleanArchitectureTemplate.Domain.UserAccounts.Events;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a permission entity in the system.
/// </summary>
public sealed class Permission : AggregateRoot<PermissionId>
{
    /// <summary>
    /// Gets the name of the permission.
    /// </summary>
    public PermissionName Name { get; private set; }

    /// <summary>
    /// Gets the description of the permission.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets the type of the permission.
    /// </summary>
    public PermissionType Type { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this permission is system-defined.
    /// System-defined permissions cannot be modified or deleted.
    /// </summary>
    public bool IsSystemDefined { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Permission"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the permission.</param>
    /// <param name="name">The name of the permission.</param>
    /// <param name="description">The description of the permission.</param>
    /// <param name="type">The type of the permission.</param>
    /// <param name="isSystemDefined">A value indicating whether this permission is system-defined.</param>
    private Permission(
        PermissionId id,
        PermissionName name,
        string description,
        PermissionType type,
        bool isSystemDefined = false) : base(id)
    {
        Name = name;
        Description = description;
        Type = type;
        IsSystemDefined = isSystemDefined;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="Permission"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the permission.</param>
    /// <param name="name">The name of the permission.</param>
    /// <param name="description">The description of the permission.</param>
    /// <param name="type">The type of the permission.</param>
    /// <param name="isSystemDefined">A value indicating whether this permission is system-defined.</param>
    /// <returns>A new <see cref="Permission"/> instance.</returns>
    public static Permission Create(
        PermissionId id,
        PermissionName name,
        string description,
        PermissionType type,
        bool isSystemDefined = false)
    {
        var permission = new Permission(id, name, description, type, isSystemDefined);
        
        // Raise domain event
        permission.AddDomainEvent(new PermissionCreatedEvent(id, name, description, type, isSystemDefined));
        
        return permission;
    }

    /// <summary>
    /// Updates the description of the permission.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result UpdateDescription(string description)
    {
        if (IsSystemDefined)
        {
            return Result.Failure(DomainError.Failure("Permission.Update", "System-defined permissions cannot be modified."));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Failure(DomainError.Failure("Permission.Description", "Description cannot be empty."));
        }

        string oldDescription = Description;
        Description = description;
        
        // Raise domain event
        AddDomainEvent(new PermissionDescriptionChangedEvent(Id, oldDescription, description));
        
        return Result.Success();
    }

    /// <summary>
    /// Updates the type of the permission.
    /// </summary>
    /// <param name="type">The new permission type.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result UpdateType(PermissionType type)
    {
        if (IsSystemDefined)
        {
            return Result.Failure(DomainError.Failure("Permission.Update", "System-defined permissions cannot be modified."));
        }

        PermissionType oldType = Type;
        Type = type;
        
        // Raise domain event
        AddDomainEvent(new PermissionTypeChangedEvent(Id, oldType, type));
        
        return Result.Success();
    }

    /// <summary>
    /// Creates a standard set of CRUD permissions for a resource.
    /// </summary>
    /// <param name="resource">The resource name (e.g., "Users", "Projects").</param>
    /// <returns>A collection of <see cref="Permission"/> instances.</returns>
    public static IEnumerable<Permission> CreateStandardPermissions(string resource)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            throw new ArgumentException("Resource name cannot be empty.", nameof(resource));
        }

        var permissions = new List<Permission>();

        // Create permission
        var createName = PermissionName.Create(resource, "Create").Value;
        permissions.Add(Create(
            PermissionId.CreateNew().Value,
            createName,
            $"Create {resource}",
            PermissionType.Create,
            true));

        // Read permission
        var readName = PermissionName.Create(resource, "Read").Value;
        permissions.Add(Create(
            PermissionId.CreateNew().Value,
            readName,
            $"Read {resource}",
            PermissionType.Read,
            true));

        // Update permission
        var updateName = PermissionName.Create(resource, "Update").Value;
        permissions.Add(Create(
            PermissionId.CreateNew().Value,
            updateName,
            $"Update {resource}",
            PermissionType.Update,
            true));

        // Delete permission
        var deleteName = PermissionName.Create(resource, "Delete").Value;
        permissions.Add(Create(
            PermissionId.CreateNew().Value,
            deleteName,
            $"Delete {resource}",
            PermissionType.Delete,
            true));

        return permissions;
    }
}
