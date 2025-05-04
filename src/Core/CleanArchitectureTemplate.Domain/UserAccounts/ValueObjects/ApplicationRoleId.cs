using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for an application role.
/// </summary>
public sealed record ApplicationRoleId : EntityId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleId"/> class.
    /// </summary>
    /// <param name="value">The value of the application role identifier.</param>
    private ApplicationRoleId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="ApplicationRoleId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the application role identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="ApplicationRoleId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<ApplicationRoleId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<ApplicationRoleId>.Failure(DomainError.Failure("ApplicationRoleId", "Application role ID cannot be empty."));
        }
        return Result<ApplicationRoleId>.Success(new ApplicationRoleId(value));
    }

    /// <summary>
    /// Creates a new <see cref="ApplicationRoleId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="ApplicationRoleId"/>.
    /// </returns>
    public static Result<ApplicationRoleId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="ApplicationRoleId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="ApplicationRoleId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator ApplicationRoleId(Guid value) => new(value);
}
