using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for an application role.
/// </summary>
public sealed record DomainRoleId : EntityId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DomainRoleId"/> class.
    /// </summary>
    /// <param name="value">The value of the application role identifier.</param>
    private DomainRoleId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="DomainRoleId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the application role identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="DomainRoleId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<DomainRoleId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<DomainRoleId>.Failure(DomainError.Failure("ApplicationRoleId", "Application role ID cannot be empty."));
        }
        return Result<DomainRoleId>.Success(new DomainRoleId(value));
    }

    /// <summary>
    /// Creates a new <see cref="DomainRoleId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="DomainRoleId"/>.
    /// </returns>
    public static Result<DomainRoleId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="DomainRoleId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="DomainRoleId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator DomainRoleId(Guid value) => new(value);
}
