namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Configuration;

/// <summary>
/// Default admin user settings.
/// </summary>
public class DefaultAdminUser
{
    /// <summary>
    /// Gets or sets the email of the default admin user.
    /// </summary>
    public string Email { get; set; } = "admin@example.com";

    /// <summary>
    /// Gets or sets the username of the default admin user.
    /// </summary>
    public string UserName { get; set; } = "admin";

    /// <summary>
    /// Gets or sets the password of the default admin user.
    /// </summary>
    public string Password { get; set; } = "Admin123!";

    /// <summary>
    /// Gets or sets the first name of the default admin user.
    /// </summary>
    public string FirstName { get; set; } = "System";

    /// <summary>
    /// Gets or sets the last name of the default admin user.
    /// </summary>
    public string LastName { get; set; } = "Administrator";
}
