using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a join entity between an application role and a permission.
/// </summary>
public sealed class RolePermission : EntityBase<RolePermissionId>
{
    /// <summary>
    /// Gets the identifier of the application role.
    /// </summary>
    public DomainRoleId RoleId { get; private set; }

    /// <summary>
    /// Gets the identifier of the permission.
    /// </summary>
    public PermissionId PermissionId { get; private set; }

    /// <summary>
    /// Gets the application role associated with this role permission.
    /// </summary>
    public DomainRole Role { get; private set; } = null!;

    /// <summary>
    /// Gets the permission associated with this role permission.
    /// </summary>
    public Permission Permission { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="RolePermission"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the role permission.</param>
    /// <param name="roleId">The identifier of the application role.</param>
    /// <param name="permissionId">The identifier of the permission.</param>
    private RolePermission(
        RolePermissionId id,
        DomainRoleId roleId,
        PermissionId permissionId) : base(id)
    {
        RoleId = roleId;
        PermissionId = permissionId;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="RolePermission"/> class.
    /// </summary>
    /// <param name="roleId">The identifier of the application role.</param>
    /// <param name="permissionId">The identifier of the permission.</param>
    /// <returns>A new <see cref="RolePermission"/> instance.</returns>
    public static RolePermission Create(
        DomainRoleId roleId,
        PermissionId permissionId)
    {
        return new RolePermission(RolePermissionId.CreateNew().Value, roleId, permissionId);
    }
}
