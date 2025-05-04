using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for a user role.
/// </summary>
public sealed record UserRoleId : EntityId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserRoleId"/> class.
    /// </summary>
    /// <param name="value">The value of the user role identifier.</param>
    private UserRoleId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="UserRoleId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the user role identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="UserRoleId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<UserRoleId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<UserRoleId>.Failure(DomainError.Failure("UserRoleId", "User role ID cannot be empty."));
        }
        return Result<UserRoleId>.Success(new UserRoleId(value));
    }

    /// <summary>
    /// Creates a new <see cref="UserRoleId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="UserRoleId"/>.
    /// </returns>
    public static Result<UserRoleId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="UserRoleId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="UserRoleId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator UserRoleId(Guid value) => new(value);
}
