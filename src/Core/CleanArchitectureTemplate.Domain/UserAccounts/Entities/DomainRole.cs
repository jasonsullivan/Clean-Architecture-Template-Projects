using CleanArchitectureTemplate.Domain.UserAccounts.Events;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents an application role in the system.
/// </summary>
public sealed class DomainRole : AggregateRoot<DomainRoleId>
{
    private readonly List<RolePermission> _permissions = [];

    /// <summary>
    /// Gets the name of the application role.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Gets the normalized name of the application role.
    /// </summary>
    public string NormalizedName { get; private set; }

    /// <summary>
    /// Gets the description of the application role.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets a value indicating whether this application role is system-defined.
    /// System-defined roles cannot be modified or deleted.
    /// </summary>
    public bool IsSystemDefined { get; private set; }

    /// <summary>
    /// Gets the permissions associated with this application role.
    /// </summary>
    public IReadOnlyCollection<RolePermission> Permissions => _permissions.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRole"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the application role.</param>
    /// <param name="name">The name of the application role.</param>
    /// <param name="normalizedName">The normalized name of the application role.</param>
    /// <param name="description">The description of the application role.</param>
    /// <param name="isSystemDefined">A value indicating whether this application role is system-defined.</param>
    private DomainRole(
        DomainRoleId id,
        string name,
        string normalizedName,
        string description,
        bool isSystemDefined = false) : base(id)
    {
        Name = name;
        NormalizedName = normalizedName;
        Description = description;
        IsSystemDefined = isSystemDefined;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="DomainRole"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the application role.</param>
    /// <param name="name">The name of the application role.</param>
    /// <param name="description">The description of the application role.</param>
    /// <param name="isSystemDefined">A value indicating whether this application role is system-defined.</param>
    /// <returns>A new <see cref="DomainRole"/> instance.</returns>
    public static DomainRole Create(
        DomainRoleId id,
        string name,
        string description,
        bool isSystemDefined = false)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Role name cannot be empty.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Role description cannot be empty.", nameof(description));
        }

        string normalizedName = name.ToUpperInvariant();
        var role = new DomainRole(id, name, normalizedName, description, isSystemDefined);

        // Raise domain event
        role.AddDomainEvent(new ApplicationRoleCreatedEvent(id, name, normalizedName, description, isSystemDefined));

        return role;
    }

    /// <summary>
    /// Updates the description of the application role.
    /// </summary>
    /// <param name="description">The new description.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result UpdateDescription(string description)
    {
        if (IsSystemDefined)
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Update", "System-defined roles cannot be modified."));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Description", "Description cannot be empty."));
        }

        Description = description;
        return Result.Success();
    }

    /// <summary>
    /// Adds a permission to the application role.
    /// </summary>
    /// <param name="permission">The permission to add.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result AddPermission(Permission permission)
    {
        if (permission == null)
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Permission", "Permission cannot be null."));
        }

        if (_permissions.Any(rp => rp.PermissionId == permission.Id))
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Permission", "Permission is already assigned to this role."));
        }

        var rolePermission = RolePermission.Create(Id, permission.Id);
        _permissions.Add(rolePermission);

        // Raise domain event
        AddDomainEvent(new ApplicationRolePermissionAddedEvent(Id, permission.Id, permission.Name));

        return Result.Success();
    }

    /// <summary>
    /// Removes a permission from the application role.
    /// </summary>
    /// <param name="permission">The permission to remove.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result RemovePermission(Permission permission)
    {
        if (permission == null)
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Permission", "Permission cannot be null."));
        }

        var rolePermission = _permissions.FirstOrDefault(rp => rp.PermissionId == permission.Id);
        if (rolePermission == null)
        {
            return Result.Failure(DomainError.Failure("ApplicationRole.Permission", "Permission is not assigned to this role."));
        }

        _permissions.Remove(rolePermission);

        // Raise domain event
        AddDomainEvent(new ApplicationRolePermissionRemovedEvent(Id, permission.Id, permission.Name));

        return Result.Success();
    }

    /// <summary>
    /// Checks if the application role has a specific permission.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission to check.</param>
    /// <returns><c>true</c> if the application role has the permission; otherwise, <c>false</c>.</returns>
    public bool HasPermission(PermissionId permissionId)
    {
        return _permissions.Any(rp => rp.PermissionId == permissionId);
    }

    /// <summary>
    /// Checks if the application role has a specific permission.
    /// </summary>
    /// <param name="permissionName">The name of the permission to check.</param>
    /// <returns><c>true</c> if the application role has the permission; otherwise, <c>false</c>.</returns>
    public bool HasPermission(PermissionName permissionName)
    {
        return _permissions.Any(rp => string.Equals(rp.Permission?.Name.Value, permissionName.Value, StringComparison.Ordinal));
    }
}
