using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

public class EFCoreIdentityIntegrationTestFixture : IAsyncLifetime, IDisposable
{
    private const string ConnectionStringTemplate =
        "Server=(localdb)\\mssqllocaldb;Database={0};Trusted_Connection=True;MultipleActiveResultSets=true";

    private readonly string _databaseName = $"IdentityTests_{Guid.NewGuid():N}";
    private readonly ServiceProvider _serviceProvider;

    public ApplicationIdentityDbContext DbContext => _serviceProvider.GetRequiredService<ApplicationIdentityDbContext>();
    public UserManager<ApplicationUser> UserManager => _serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    public RoleManager<ApplicationRole> RoleManager => _serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

    public EFCoreIdentityIntegrationTestFixture()
    {
        var services = new ServiceCollection();

        string connectionString = string.Format(ConnectionStringTemplate, _databaseName);

        // Register DbContext
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Add logging - important for Identity services
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

        // Add HTTP context accessor (needed for CurrentUserService)
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        _serviceProvider = services.BuildServiceProvider();
    }

    public async Task InitializeAsync()
    {
        // Create the database and apply migrations
        await DbContext.Database.EnsureCreatedAsync();

        // Ensure test data is seeded during initialization
        await IdentityTestDataSeeder.SeedBasicUserAsync(UserManager, RoleManager);
        await IdentityTestDataSeeder.SeedRolesWithPermissionsAsync(RoleManager);
    }

    public async Task DisposeAsync()
    {
        // Clean up database after tests
        await DbContext.Database.EnsureDeletedAsync();
    }

    public void Dispose()
    {
        _serviceProvider?.Dispose();
    }
}