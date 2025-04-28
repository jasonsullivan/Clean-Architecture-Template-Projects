using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a person's name, including first, last, and optional middle name.
/// </summary>
public sealed record PersonName : ValueObject
{
    /// <summary>
    /// Gets the first name of the person.
    /// </summary>
    public string FirstName { get; init; }

    /// <summary>
    /// Gets the last name of the person.
    /// </summary>
    public string LastName { get; init; }

    /// <summary>
    /// Gets the middle name of the person, if any.
    /// </summary>
    public string? MiddleName { get; init; }

    /// <summary>
    /// Gets the full name of the person.
    /// </summary>
    public string FullName => $"{FirstName} {MiddleName ?? ""} {LastName}".Trim();

    /// <summary>
    /// Gets the initials of the person's name.
    /// </summary>
    public string Initials => $"{FirstNameInitial}{MiddleNameInitial ?? ""}{LastNameInitial}".ToUpperInvariant();

    /// <summary>
    /// Gets the initial of the first name.
    /// </summary>
    public string FirstNameInitial => $"{FirstName[0]}".ToUpperInvariant();

    /// <summary>
    /// Gets the initial of the last name.
    /// </summary>
    public string LastNameInitial => $"{LastName[0]}".ToUpperInvariant();

    /// <summary>
    /// Gets the initial of the middle name, if any.
    /// </summary>
    public string MiddleNameInitial => $"{MiddleName?[0]}".ToUpperInvariant();

    /// <summary>
    /// Gets the formatted name as "FirstName MiddleInitial LastName".
    /// </summary>
    public string FirstNameMiddleNameInitialLastName => $"{FirstName} {(MiddleNameInitial ?? "").ToUpperInvariant()} {LastName}";

    /// <summary>
    /// Initializes a new instance of the <see cref="PersonName"/> record.
    /// </summary>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>
    /// <param name="middleName">The middle name of the person, if any.</param>
    private PersonName(string firstName, string lastName, string? middleName = null)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
    }

    /// <summary>
    /// Creates a <see cref="PersonName"/> instance from the provided names.
    /// </summary>
    /// <param name="firstName">The first name of the person.</param>
    /// <param name="lastName">The last name of the person.</param>
    /// <param name="middleName">The middle name of the person, if any.</param>
    /// <returns>
    /// A <see cref="Result{TValue}"/> containing the created <see cref="PersonName"/> if successful,
    /// or a failure result with a <see cref="DomainError"/> if the names are invalid.
    /// </returns>
    public static Result<PersonName> Create(string firstName, string lastName, string? middleName = null)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
        {
            return Result<PersonName>.Failure(DomainError.Failure("PersonName", "First name and last name cannot be empty."));
        }
        return Result<PersonName>.Success(new PersonName(firstName, lastName, middleName));
    }
}
