using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a join entity between a user account and an application role.
/// </summary>
public sealed class UserRole : EntityBase<UserRoleId>
{
    /// <summary>
    /// Gets the identifier of the user account.
    /// </summary>
    public UserAccountId UserAccountId { get; private set; }

    /// <summary>
    /// Gets the identifier of the application role.
    /// </summary>
    public ApplicationRoleId RoleId { get; private set; }

    /// <summary>
    /// Gets the user account associated with this user role.
    /// </summary>
    public UserAccount UserAccount { get; private set; } = null!;

    /// <summary>
    /// Gets the application role associated with this user role.
    /// </summary>
    public ApplicationRole Role { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRole"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user role.</param>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleId">The identifier of the application role.</param>
    private UserRole(
        UserRoleId id,
        UserAccountId userAccountId,
        ApplicationRoleId roleId) : base(id)
    {
        UserAccountId = userAccountId;
        RoleId = roleId;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="UserRole"/> class.
    /// </summary>
    /// <param name="userAccountId">The identifier of the user account.</param>
    /// <param name="roleId">The identifier of the application role.</param>
    /// <returns>A new <see cref="UserRole"/> instance.</returns>
    public static UserRole Create(
        UserAccountId userAccountId,
        ApplicationRoleId roleId)
    {
        return new UserRole(UserRoleId.CreateNew().Value, userAccountId, roleId);
    }
}
