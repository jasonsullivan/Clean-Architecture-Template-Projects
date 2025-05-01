using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

using Microsoft.AspNetCore.Identity;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

public static class IdentityTestDataSeeder
{
    public static async Task SeedBasicUserAsync(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roles = { "Administrator", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new ApplicationRole(roleName));
            }
        }

        // Create a test user
        var testUser = new ApplicationUser
        {
            UserName = "testuser",
            Email = "test@example.com",
            EmailConfirmed = true,
            FirstName = "Test",
            LastName = "User",
            DomainId = System.Guid.NewGuid().ToString()
        };

        if (await userManager.FindByNameAsync(testUser.UserName) == null)
        {
            await userManager.CreateAsync(testUser, "Password123!");
            await userManager.AddToRoleAsync(testUser, "User");
        }

        // Create an admin user
        var adminUser = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@example.com",
            EmailConfirmed = true,
            FirstName = "Admin",
            LastName = "User",
            DomainId = System.Guid.NewGuid().ToString()
        };

        if (await userManager.FindByNameAsync(adminUser.UserName) == null)
        {
            await userManager.CreateAsync(adminUser, "Admin123!");
            await userManager.AddToRoleAsync(adminUser, "Administrator");
        }
    }

    public static async Task SeedRolesWithPermissionsAsync(
        RoleManager<ApplicationRole> roleManager)
    {
        // Create roles with permissions
        var roles = new Dictionary<string, List<string>>
        {
            ["ContentEditor"] = new List<string> { "Content.Create", "Content.Edit", "Content.View" },
            ["ContentViewer"] = new List<string> { "Content.View" },
            ["UserManager"] = new List<string> { "Users.Create", "Users.Edit", "Users.Delete", "Users.View" }
        };

        foreach (var role in roles)
        {
            // Create role if it doesn't exist
            if (!await roleManager.RoleExistsAsync(role.Key))
            {
                var newRole = new ApplicationRole(role.Key);
                await roleManager.CreateAsync(newRole);

                // Add permissions as claims
                foreach (var permission in role.Value)
                {
                    await roleManager.AddClaimAsync(newRole,
                        new System.Security.Claims.Claim("Permission", permission));
                }
            }
        }
    }
}