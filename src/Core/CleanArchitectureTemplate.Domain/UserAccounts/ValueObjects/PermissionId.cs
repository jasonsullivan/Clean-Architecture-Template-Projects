using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for a permission.
/// </summary>
public sealed record PermissionId : EntityId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionId"/> class.
    /// </summary>
    /// <param name="value">The value of the permission identifier.</param>
    private PermissionId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="PermissionId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the permission identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="PermissionId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<PermissionId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<PermissionId>.Failure(DomainError.Failure("PermissionId", "Permission ID cannot be empty."));
        }
        return Result<PermissionId>.Success(new PermissionId(value));
    }

    /// <summary>
    /// Creates a new <see cref="PermissionId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="PermissionId"/>.
    /// </returns>
    public static Result<PermissionId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="PermissionId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="PermissionId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator PermissionId(Guid value) => new(value);
}
