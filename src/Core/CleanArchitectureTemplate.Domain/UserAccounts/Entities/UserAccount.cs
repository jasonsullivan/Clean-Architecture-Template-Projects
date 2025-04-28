using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Entities;

/// <summary>
/// Represents a user account in the system.
/// </summary>
public sealed class UserAccount : AggregateRoot<UserAccountId>
{
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
}
