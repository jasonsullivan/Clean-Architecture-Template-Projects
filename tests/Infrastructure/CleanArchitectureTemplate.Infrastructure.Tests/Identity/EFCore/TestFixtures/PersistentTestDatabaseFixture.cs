using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

public class PersistentTestDatabaseFixture : IAsyncLifetime
{
    // Use a fixed database name for investigation
    public const string DatabaseName = "IdentityTestsInvestigation";
    public const string ConnectionString =
        @"Server=(localdb)\mssqllocaldb;Database=IdentityTestsInvestigation;Trusted_Connection=True;MultipleActiveResultSets=true";

    private readonly ServiceProvider _serviceProvider;

    public ApplicationIdentityDbContext DbContext => _serviceProvider.GetRequiredService<ApplicationIdentityDbContext>();
    public UserManager<ApplicationUser> UserManager => _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    public RoleManager<ApplicationRole> RoleManager => _serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    public PersistentTestDatabaseFixture()
    {
        var services = new ServiceCollection();

        // Register DbContext
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(ConnectionString));

        // Add logging
        services.AddLogging(builder =>
        {
            builder.AddDebug();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        // Add Identity services with properly configured stores
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Configure Identity options for testing
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequireUppercase = true;
            options.Password.RequiredLength = 8;

            // Disable lockout for testing
            options.Lockout.AllowedForNewUsers = false;

            // Relax email requirements for testing
            options.User.RequireUniqueEmail = true;
            options.SignIn.RequireConfirmedEmail = false;
        })
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddDefaultTokenProviders();

        // Add HTTP context accessor
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        Console.WriteLine($"Initializing persistent test database: {DatabaseName}");

        // Create or ensure the database exists
        await DbContext.Database.EnsureCreatedAsync();

        // Clean existing data if needed
        await CleanDatabaseAsync();

        // Seed fresh test data
        await IdentityTestDataSeeder.SeedBasicUserAsync(UserManager, RoleManager);
        await IdentityTestDataSeeder.SeedRolesWithPermissionsAsync(RoleManager);

        Console.WriteLine($"Persistent test database ready at: {ConnectionString}");
        Console.WriteLine($"Use SQL Server Object Explorer to connect to (localdb)\\MSSQLLocalDB and examine the {DatabaseName} database");
    }

    private async Task CleanDatabaseAsync()
    {
        // Clean existing data to start fresh
        // Respect foreign key constraints by deleting in correct order
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM UserTokens");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM UserLogins");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM UserClaims");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM UserRoles");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM RoleClaims");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM Users");
        await DbContext.Database.ExecuteSqlRawAsync("DELETE FROM Roles");
    }

    public Task DisposeAsync()
    {
        // Don't delete the database, just dispose resources
        _serviceProvider?.Dispose();
        return Task.CompletedTask;
    }
}