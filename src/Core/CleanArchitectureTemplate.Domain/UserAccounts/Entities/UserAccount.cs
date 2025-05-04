using CleanArchitectureTemplate.Domain.UserAccounts.Events;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public sealed class UserAccount : AggregateRoot<UserAccountId>
{
    private readonly List<UserRole> _roles = [];

    /// <summary>
    /// Gets the username of the user account.
    /// </summary>
    public UserName Username { get; private set; }

    /// <summary>
    /// Gets the email address associated with the user account.
    /// </summary>
    public Email Email { get; private set; }

    /// <summary>
    /// Gets the current status of the user account.
    /// </summary>
    public UserStatus Status { get; private set; }

    /// <summary>
    /// Gets the full name of the person associated with the user account.
    /// </summary>
    public PersonName PersonName { get; private set; }

    /// <summary>
    /// Gets the phone number of the user, if available.
    /// </summary>
    public PhoneNumber? PhoneNumber { get; private set; }

    /// <summary>
    /// Gets the roles associated with this user account.
    /// </summary>
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user account.</param>
    /// <param name="username">The username of the user account.</param>
    /// <param name="email">The email address associated with the user account.</param>
    /// <param name="status">The current status of the user account.</param>
    /// <param name="personName">The full name of the person associated with the user account.</param>
    /// <param name="phoneNumber">The phone number of the user, if available.</param>
    private UserAccount(
        UserAccountId id,
        UserName username,
        Email email,
        UserStatus status,
        PersonName personName,
        PhoneNumber? phoneNumber = null) : base(id)
    {
        Username = username;
        Email = email;
        Status = status;
        PersonName = personName;
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Creates a new instance of the <see cref="UserAccount"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for the user account.</param>
    /// <param name="username">The username of the user account.</param>
    /// <param name="email">The email address associated with the user account.</param>
    /// <param name="status">The current status of the user account.</param>
    /// <param name="personName">The full name of the person associated with the user account.</param>
    /// <param name="phoneNumber">The phone number of the user, if available.</param>
    /// <returns>A new <see cref="UserAccount"/> instance.</returns>
    public static UserAccount Create(
        UserAccountId id,
        UserName username,
        Email email,
        UserStatus status,
        PersonName personName,
        PhoneNumber? phoneNumber = null)
    {
        return new UserAccount(id, username, email, status, personName, phoneNumber);
    }

    /// <summary>
    /// Updates the email address associated with the user account.
    /// </summary>
    /// <param name="email">The new email address.</param>
    public void UpdateEmail(Email email)
    {
        Email = email;
    }

    /// <summary>
    /// Updates the current status of the user account.
    /// </summary>
    /// <param name="status">The new status.</param>
    public void UpdateStatus(UserStatus status)
    {
        Status = status;
    }

    /// <summary>
    /// Updates the full name of the person associated with the user account.
    /// </summary>
    /// <param name="personName">The new full name.</param>
    public void UpdatePersonName(PersonName personName)
    {
        PersonName = personName;
    }

    /// <summary>
    /// Updates the phone number of the user.
    /// </summary>
    /// <param name="phoneNumber">The new phone number.</param>
    public void UpdatePhoneNumber(PhoneNumber phoneNumber)
    {
        PhoneNumber = phoneNumber;
    }

    /// <summary>
    /// Updates the username of the user account.
    /// </summary>
    /// <param name="username">The new username.</param>
    public void UpdateUsername(UserName username)
    {
        Username = username;
    }

    /// <summary>
    /// Adds a role to the user account.
    /// </summary>
    /// <param name="role">The role to add.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result AddRole(ApplicationRole role)
    {
        if (role == null)
        {
            return Result.Failure(DomainError.Failure("UserAccount.Role", "Role cannot be null."));
        }

        if (_roles.Any(ur => ur.RoleId == role.Id))
        {
            return Result.Failure(DomainError.Failure("UserAccount.Role", "Role is already assigned to this user."));
        }

        var userRole = UserRole.Create(Id, role.Id);
        _roles.Add(userRole);

        // Raise domain event
        AddDomainEvent(new UserAccountRoleAddedEvent(Id, role.Id, role.Name));

        return Result.Success();
    }

    /// <summary>
    /// Removes a role from the user account.
    /// </summary>
    /// <param name="role">The role to remove.</param>
    /// <returns>A <see cref="Result"/> indicating success or failure.</returns>
    public Result RemoveRole(ApplicationRole role)
    {
        if (role == null)
        {
            return Result.Failure(DomainError.Failure("UserAccount.Role", "Role cannot be null."));
        }

        var userRole = _roles.FirstOrDefault(ur => ur.RoleId == role.Id);
        if (userRole == null)
        {
            return Result.Failure(DomainError.Failure("UserAccount.Role", "Role is not assigned to this user."));
        }

        _roles.Remove(userRole);

        // Raise domain event
        AddDomainEvent(new UserAccountRoleRemovedEvent(Id, role.Id, role.Name));

        return Result.Success();
    }

    /// <summary>
    /// Checks if the user account has a specific role.
    /// </summary>
    /// <param name="roleId">The identifier of the role to check.</param>
    /// <returns><c>true</c> if the user account has the role; otherwise, <c>false</c>.</returns>
    public bool HasRole(ApplicationRoleId roleId)
    {
        return _roles.Any(ur => ur.RoleId == roleId);
    }

    /// <summary>
    /// Checks if the user account has a specific role.
    /// </summary>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns><c>true</c> if the user account has the role; otherwise, <c>false</c>.</returns>
    public bool HasRole(string roleName)
    {
        return _roles.Any(ur => string.Equals(ur.Role?.NormalizedName, roleName, StringComparison.InvariantCultureIgnoreCase));
    }

    /// <summary>
    /// Checks if the user account has a specific permission.
    /// </summary>
    /// <param name="permissionId">The identifier of the permission to check.</param>
    /// <returns><c>true</c> if the user account has the permission; otherwise, <c>false</c>.</returns>
    public bool HasPermission(PermissionId permissionId)
    {
        return _roles.Any(ur => ur.Role?.HasPermission(permissionId) == true);
    }

    /// <summary>
    /// Checks if the user account has a specific permission.
    /// </summary>
    /// <param name="permissionName">The name of the permission to check.</param>
    /// <returns><c>true</c> if the user account has the permission; otherwise, <c>false</c>.</returns>
    public bool HasPermission(PermissionName permissionName)
    {
        return _roles.Any(ur => ur.Role?.HasPermission(permissionName) == true);
    }
}
