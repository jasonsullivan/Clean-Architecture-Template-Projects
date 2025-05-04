using CleanArchitectureTemplate.Shared.Primitives;

using System.Text.RegularExpressions;

namespace CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;

/// <summary>
/// Represents a permission name value object in the domain.
/// Permission names follow the format "Resource.Action" (e.g., "Users.Create", "Projects.Read").
/// </summary>
public sealed partial record PermissionName : ValueObject
{
    /// <summary>
    /// Gets the permission name value.
    /// </summary>
    public string Value { get; init; }

    /// <summary>
    /// Gets the resource part of the permission name (e.g., "Users" from "Users.Create").
    /// </summary>
    public string Resource => Value.Split('.')[0];

    /// <summary>
    /// Gets the action part of the permission name (e.g., "Create" from "Users.Create").
    /// </summary>
    public string Action => Value.Split('.').Length > 1 ? Value.Split('.')[1] : string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionName"/> class with the specified permission name value.
    /// </summary>
    /// <param name="value">The permission name value.</param>
    private PermissionName(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new <see cref="PermissionName"/> instance after validating the provided permission name string.
    /// </summary>
    /// <param name="value">The permission name string to validate and create the value object from.</param>
    /// <returns>
    /// A <see cref="Result{PermissionName}"/> containing the created <see cref="PermissionName"/> instance if valid,
    /// or a failure result with the appropriate domain error.
    /// </returns>
    public static Result<PermissionName> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<PermissionName>.Failure(DomainError.Failure("PermissionName", "Permission name cannot be empty."));
        }

        if (!PermissionNameValidationRegex().IsMatch(value))
        {
            return Result<PermissionName>.Failure(DomainError.Failure("PermissionName", 
                "Invalid permission name format. Permission name must follow the format 'Resource.Action' (e.g., 'Users.Create')."));
        }

        return Result<PermissionName>.Success(new PermissionName(value));
    }

    /// <summary>
    /// Creates a new <see cref="PermissionName"/> instance by combining a resource and an action.
    /// </summary>
    /// <param name="resource">The resource part of the permission name (e.g., "Users").</param>
    /// <param name="action">The action part of the permission name (e.g., "Create").</param>
    /// <returns>
    /// A <see cref="Result{PermissionName}"/> containing the created <see cref="PermissionName"/> instance if valid,
    /// or a failure result with the appropriate domain error.
    /// </returns>
    public static Result<PermissionName> Create(string resource, string action)
    {
        if (string.IsNullOrWhiteSpace(resource))
        {
            return Result<PermissionName>.Failure(DomainError.Failure("PermissionName", "Resource name cannot be empty."));
        }

        if (string.IsNullOrWhiteSpace(action))
        {
            return Result<PermissionName>.Failure(DomainError.Failure("PermissionName", "Action name cannot be empty."));
        }

        return Create($"{resource}.{action}");
    }

    /// <summary>
    /// Provides the regular expression used to validate permission names.
    /// </summary>
    /// <returns>A <see cref="Regex"/> instance for permission name validation.</returns>
    [GeneratedRegex(@"^[A-Za-z0-9]+\.[A-Za-z0-9]+$")]
    private static partial Regex PermissionNameValidationRegex();

    /// <summary>
    /// Returns a string representation of the permission name.
    /// </summary>
    /// <returns>The permission name value.</returns>
    public override string ToString() => Value;
}
