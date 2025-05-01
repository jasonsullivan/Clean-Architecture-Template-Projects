using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Base;

/// <summary>
/// Provides a base class for testing EF Core Identity functionality.
/// </summary>
public class EFCoreIdentityTestBase
{
    /// <summary>
    /// Creates a new instance of <see cref="ApplicationIdentityDbContext"/> configured to use an in-memory database.
    /// </summary>
    /// <returns>A new instance of <see cref="ApplicationIdentityDbContext"/>.</returns>
    protected ApplicationIdentityDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationIdentityDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // New DB per test
            .Options;

        var context = new ApplicationIdentityDbContext(options);

        // Ensure the model is created with all entity types
        context.Database.EnsureCreated();

        return context;
    }

    /// <summary>
    /// Creates instances of <see cref="UserManager{TUser}"/>, <see cref="RoleManager{TRole}"/>, and <see cref="ApplicationIdentityDbContext"/> for testing purposes.
    /// </summary>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    /// <item><description>An instance of <see cref="UserManager{TUser}"/> for managing users.</description></item>
    /// <item><description>An instance of <see cref="RoleManager{TRole}"/> for managing roles.</description></item>
    /// <item><description>An instance of <see cref="ApplicationIdentityDbContext"/> for database operations.</description></item>
    /// </list>
    /// </returns>
    protected (UserManager<ApplicationUser>, RoleManager<ApplicationRole>, ApplicationIdentityDbContext) CreateUserAndRoleManagers()
    {
        // Create DbContext
        var dbContext = CreateDbContext();

        // Set up the stores with proper configuration
        var userStore = new UserStore<ApplicationUser, ApplicationRole, ApplicationIdentityDbContext,
            string, ApplicationUserClaim, ApplicationUserRole, ApplicationUserLogin,
            ApplicationUserToken, ApplicationRoleClaim>(dbContext);

        var roleStore = new RoleStore<ApplicationRole, ApplicationIdentityDbContext,
            string, ApplicationUserRole, ApplicationRoleClaim>(dbContext);

        // Create the managers with required services
        var userManager = new UserManager<ApplicationUser>(
            userStore,
            Options.Create(new IdentityOptions()),
            new PasswordHasher<ApplicationUser>(),
            new List<IUserValidator<ApplicationUser>>
            {
                new UserValidator<ApplicationUser>()
            },
            new List<IPasswordValidator<ApplicationUser>>
            {
                new PasswordValidator<ApplicationUser>()
            },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            null,
            new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

        var roleManager = new RoleManager<ApplicationRole>(
            roleStore,
            new List<IRoleValidator<ApplicationRole>>
            {
                new RoleValidator<ApplicationRole>()
            },
            new UpperInvariantLookupNormalizer(),
            new IdentityErrorDescriber(),
            new Mock<ILogger<RoleManager<ApplicationRole>>>().Object);

        return (userManager, roleManager, dbContext);
    }
}
