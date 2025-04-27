using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a unique identifier for a user account.
/// </summary>
public sealed record UserAccountId : UserId<Guid>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountId"/> class.
    /// </summary>
    /// <param name="value">The value of the user account identifier.</param>
    private UserAccountId(Guid value) : base(value) { }

    /// <summary>
    /// Creates a new <see cref="UserAccountId"/> instance with the specified GUID value.
    /// </summary>
    /// <param name="value">The GUID value to initialize the user account identifier.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="UserAccountId"/> if successful,
    /// or a failure result if the provided GUID is invalid.
    /// </returns>
    public static Result<UserAccountId> Create(Guid value)
    {
        if (value == Guid.Empty)
        {
            return Result<UserAccountId>.Failure(DomainError.Failure("UserAccountId", "User account ID cannot be empty."));
        }
        return Result<UserAccountId>.Success(new UserAccountId(value));
    }

    /// <summary>
    /// Creates a new <see cref="UserAccountId"/> instance with a newly generated GUID value.
    /// </summary>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="UserAccountId"/>.
    /// </returns>
    public static Result<UserAccountId> CreateNew()
    {
        return Create(Guid.NewGuid());
    }

    /// <summary>
    /// Implicitly converts a <see cref="Guid"/> to a <see cref="UserAccountId"/>.
    /// </summary>
    /// <param name="value">The GUID value to convert.</param>
    /// <returns>
    /// A new instance of <see cref="UserAccountId"/> initialized with the specified GUID value.
    /// </returns>
    public static implicit operator UserAccountId(Guid value) => new(value);
}

