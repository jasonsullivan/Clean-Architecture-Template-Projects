using CleanArchitectureTemplate.Application.Common.Interfaces.Identity;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Configuration;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using System.Text;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Extensions;

/// <summary>
/// Extension methods for configuring EF Core Identity services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds EF Core Identity services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection with Identity services added.</returns>
    public static IServiceCollection AddEFCoreIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind the EF Core Identity options from configuration
        var efCoreIdentityOptionsSection = configuration.GetSection("Identity:EFCoreIdentity");
        services.Configure<EFCoreIdentityOptions>(efCoreIdentityOptionsSection);

        var efCoreIdentityOptions = efCoreIdentityOptionsSection.Get<EFCoreIdentityOptions>() ?? new EFCoreIdentityOptions();

        // Configure the database context
        services.AddDbContext<ApplicationIdentityDbContext>(options =>
        {
            switch (efCoreIdentityOptions.DatabaseProvider.ToLower())
            {
                case "postgresql":
                    options.UseNpgsql(
                        efCoreIdentityOptions.ConnectionString,
                        b => b.MigrationsAssembly("CleanArchitectureTemplate.Infrastructure.Identity.EFCore"));
                    break;
                case "sqlite":
                    options.UseSqlite(
                        efCoreIdentityOptions.ConnectionString,
                        b => b.MigrationsAssembly("CleanArchitectureTemplate.Infrastructure.Identity.EFCore"));
                    break;
                case "sqlserver":
                default:
                    options.UseSqlServer(
                        efCoreIdentityOptions.ConnectionString,
                        b => b.MigrationsAssembly("CleanArchitectureTemplate.Infrastructure.Identity.EFCore"));
                    break;
            }
        });

        // Configure ASP.NET Core Identity
        var identityBuilder = services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            // Password settings
            options.Password.RequireDigit = efCoreIdentityOptions.PasswordOptions.RequireDigit;
            options.Password.RequireLowercase = efCoreIdentityOptions.PasswordOptions.RequireLowercase;
            options.Password.RequireNonAlphanumeric = efCoreIdentityOptions.PasswordOptions.RequireNonAlphanumeric;
            options.Password.RequireUppercase = efCoreIdentityOptions.PasswordOptions.RequireUppercase;
            options.Password.RequiredLength = efCoreIdentityOptions.PasswordOptions.RequiredLength;
            options.Password.RequiredUniqueChars = efCoreIdentityOptions.PasswordOptions.RequiredUniqueChars;

            // User settings
            options.User.RequireUniqueEmail = efCoreIdentityOptions.UserOptions.RequireUniqueEmail;
            options.SignIn.RequireConfirmedEmail = efCoreIdentityOptions.UserOptions.RequireEmailConfirmation;
            options.SignIn.RequireConfirmedPhoneNumber = efCoreIdentityOptions.UserOptions.RequirePhoneNumberConfirmation;

            // Lockout settings
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(efCoreIdentityOptions.LockoutOptions.LockoutTimeSpanInMinutes);
            options.Lockout.MaxFailedAccessAttempts = efCoreIdentityOptions.LockoutOptions.MaxFailedAccessAttempts;
            options.Lockout.AllowedForNewUsers = efCoreIdentityOptions.LockoutOptions.EnableLockout;
        })
        .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
        .AddDefaultTokenProviders();

        // Register the mapper with logger
        services.AddSingleton<IdentityMapper>(sp =>
            new IdentityMapper(sp.GetRequiredService<ILogger<IdentityMapper>>()));

        // Register the identity services
        services.AddScoped<EFCoreIdentityService>();
        services.AddScoped<EFCoreCurrentUserService>();
        services.AddScoped<IIdentityProviderFactory, EFCoreIdentityProviderFactory>();

        // Add logging
        services.AddLogging();

        return services;
    }

    /// <summary>
    /// Adds JWT authentication for API endpoints.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection with JWT authentication added.</returns>
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettingsSection = configuration.GetSection("Identity:JwtSettings");

        var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? new JwtSettings();

        services.Configure<JwtSettings>(jwtSettingsSection);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
                ClockSkew = TimeSpan.Zero
            };
        });

        return services;
    }

    /// <summary>
    /// Adds cookie authentication for web applications.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns>The service collection with cookie authentication added.</returns>
    public static IServiceCollection AddCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var cookieSettingsSection = configuration.GetSection("Identity:CookieSettings");

        var cookieSettings = cookieSettingsSection.Get<CookieSettings>() ?? new CookieSettings();

        services.Configure<CookieSettings>(cookieSettingsSection);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
            options.ExpireTimeSpan = TimeSpan.FromMinutes(cookieSettings.ExpirationTimeInMinutes);
            options.SlidingExpiration = cookieSettings.SlidingExpiration;
            options.LoginPath = cookieSettings.LoginPath;
            options.LogoutPath = cookieSettings.LogoutPath;
            options.AccessDeniedPath = cookieSettings.AccessDeniedPath;
        });

        return services;
    }

    /// <summary>
    /// Ensures that the identity database is created and migrated.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder.</returns>
    public static IApplicationBuilder UseEFCoreIdentityMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var efCoreIdentityOptions = configuration.GetSection("Identity:EFCoreIdentity").Get<EFCoreIdentityOptions>() ?? new EFCoreIdentityOptions();

        // Apply migrations
        dbContext.Database.Migrate();

        // Seed default data if enabled
        if (efCoreIdentityOptions.SeedDefaultData)
        {
            // Seed default roles
            SeedDefaultRoles(roleManager).GetAwaiter().GetResult();

            // Seed default admin user
            SeedDefaultAdminUser(userManager, roleManager, configuration).GetAwaiter().GetResult();
        }

        return app;
    }

    private static async Task SeedDefaultRoles(RoleManager<ApplicationRole> roleManager)
    {
        // Seed default roles if they don't exist
        string[] defaultRoles = ["Administrator", "User"];

        foreach (var roleName in defaultRoles)
        {
            if (!await roleManager.RoleExistsAsync(roleName).ConfigureAwait(false))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName)).ConfigureAwait(false);
            }
        }
    }

    private static async Task SeedDefaultAdminUser(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        IConfiguration configuration)
    {
        // Get admin user configuration
        var adminUserConfig = configuration.GetSection("Identity:DefaultAdminUser").Get<DefaultAdminUser>() ?? new DefaultAdminUser();

        // Check if admin user exists
        var adminUser = await userManager.FindByEmailAsync(adminUserConfig.Email).ConfigureAwait(false);

        if (adminUser == null)
        {
            // Create admin user
            adminUser = new ApplicationUser
            {
                UserName = adminUserConfig.UserName,
                Email = adminUserConfig.Email,
                EmailConfirmed = true,
                FirstName = adminUserConfig.FirstName,
                LastName = adminUserConfig.LastName
            };

            var result = await userManager.CreateAsync(adminUser, adminUserConfig.Password).ConfigureAwait(false);

            if (result.Succeeded)
            {
                // Assign admin role
                await userManager.AddToRoleAsync(adminUser, "Administrator").ConfigureAwait(false);
            }
        }
    }
}
