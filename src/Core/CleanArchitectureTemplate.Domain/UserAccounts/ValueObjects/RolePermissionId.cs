using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for a role permission.
/// </summary>
public sealed record RolePermissionId : EntityId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RolePermissionId"/> class.
    /// </summary>
    /// <param name="value">The value of the role permission identifier.</param>
    private RolePermissionId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="RolePermissionId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the role permission identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="RolePermissionId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<RolePermissionId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<RolePermissionId>.Failure(DomainError.Failure("RolePermissionId", "Role permission ID cannot be empty."));
        }
        return Result<RolePermissionId>.Success(new RolePermissionId(value));
    }

    /// <summary>
    /// Creates a new <see cref="RolePermissionId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="RolePermissionId"/>.
    /// </returns>
    public static Result<RolePermissionId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="RolePermissionId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="RolePermissionId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator RolePermissionId(Guid value) => new(value);
}
