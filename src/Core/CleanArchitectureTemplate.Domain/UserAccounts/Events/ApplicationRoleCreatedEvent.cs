using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Shared.Primitives;

namespace CleanArchitectureTemplate.Domain.UserAccounts.Events;

/// <summary>
/// Represents an event that is raised when an application role is created.
/// </summary>
public sealed class ApplicationRoleCreatedEvent : DomainEvent
{
    /// <summary>
    /// Gets the identifier of the application role that was created.
    /// </summary>
    public ApplicationRoleId RoleId { get; }

    /// <summary>
    /// Gets the name of the application role that was created.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the normalized name of the application role that was created.
    /// </summary>
    public string NormalizedName { get; }

    /// <summary>
    /// Gets the description of the application role that was created.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets a value indicating whether the application role is system-defined.
    /// </summary>
    public bool IsSystemDefined { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationRoleCreatedEvent"/> class.
    /// </summary>
    /// <param name="roleId">The identifier of the application role that was created.</param>
    /// <param name="name">The name of the application role that was created.</param>
    /// <param name="normalizedName">The normalized name of the application role that was created.</param>
    /// <param name="description">The description of the application role that was created.</param>
    /// <param name="isSystemDefined">A value indicating whether the application role is system-defined.</param>
    public ApplicationRoleCreatedEvent(
        ApplicationRoleId roleId,
        string name,
        string normalizedName,
        string description,
        bool isSystemDefined)
    {
        RoleId = roleId;
        Name = name;
        NormalizedName = normalizedName;
        Description = description;
        IsSystemDefined = isSystemDefined;
    }
}
