namespace CleanArchitectureTemplate.Domain.UserAccounts.Enums;

/// <summary>
/// Represents the type of permission action.
/// </summary>
public enum PermissionType
{
    /// <summary>
    /// Permission to create resources.
    /// </summary>
    Create = 1,

    /// <summary>
    /// Permission to read or view resources.
    /// </summary>
    Read = 2,

    /// <summary>
    /// Permission to update or modify resources.
    /// </summary>
    Update = 3,

    /// <summary>
    /// Permission to delete resources.
    /// </summary>
    Delete = 4,

    /// <summary>
    /// Permission to execute operations or processes.
    /// </summary>
    Execute = 5,

    /// <summary>
    /// Permission to manage or administer resources.
    /// </summary>
    Manage = 6,

    /// <summary>
    /// Full control over resources.
    /// </summary>
    FullControl = 7
}
