using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

using Microsoft.Extensions.Logging;

using Moq;

using System.Security.Claims;

using Xunit.Abstractions;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Investigation;

[Collection("PersistentDatabaseTests")]
public class DatabaseInvestigationTests : IClassFixture<PersistentTestDatabaseFixture>
{
    private readonly PersistentTestDatabaseFixture _fixture;
    private readonly EFCoreIdentityService _identityService;
    private readonly IdentityMapper _mapper;
    private readonly ITestOutputHelper _output;

    public DatabaseInvestigationTests(PersistentTestDatabaseFixture fixture, ITestOutputHelper output)
    {
        _fixture = fixture;
        _output = output;

        // Create mapper
        var mapperLogger = new Mock<ILogger<IdentityMapper>>();
        _mapper = new IdentityMapper(mapperLogger.Object);

        // Create identity service
        var serviceLogger = new Mock<ILogger<EFCoreIdentityService>>();
        _identityService = new EFCoreIdentityService(
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _mapper,
            serviceLogger.Object);
    }

    [Fact]
    public async Task PopulateDatabaseForInvestigation()
    {
        _output.WriteLine("Creating test data for inspection...");

        // Create a complex set of users, roles, and permissions
        await CreateTestDataAsync();

        // Output helpful information
        _output.WriteLine("");
        _output.WriteLine("Test data has been created in the database.");
        _output.WriteLine($"Database name: {PersistentTestDatabaseFixture.DatabaseName}");
        _output.WriteLine($"Connection string: {PersistentTestDatabaseFixture.ConnectionString}");
        _output.WriteLine("");
        _output.WriteLine("Suggested SQL queries for inspection:");
        _output.WriteLine("1. SELECT * FROM Users");
        _output.WriteLine("2. SELECT * FROM Roles");
        _output.WriteLine("3. SELECT u.UserName, r.Name as RoleName FROM Users u JOIN UserRoles ur ON u.Id = ur.UserId JOIN Roles r ON ur.RoleId = r.Id");
        _output.WriteLine("4. SELECT r.Name as RoleName, rc.ClaimValue as Permission FROM Roles r JOIN RoleClaims rc ON r.Id = rc.RoleId WHERE rc.ClaimType = 'Permission'");
        _output.WriteLine("");
        _output.WriteLine("The database has been populated and will remain available until manually deleted.");
        _output.WriteLine("You can now open SQL Server Object Explorer and connect to investigate the data.");
    }

    private async Task CreateTestDataAsync()
    {
        // Create various users
        var users = new List<(string userName, string email, string firstName, string lastName, string password)>
        {
            ("developer", "developer@example.com", "Developer", "User", "Dev123!@#"),
            ("manager", "manager@example.com", "Manager", "User", "Manager123!@#"),
            ("admin", "admin@example.com", "Admin", "User", "Admin123!@#"),
            ("readonly", "readonly@example.com", "ReadOnly", "User", "ReadOnly123!@#"),
            ("suspended", "suspended@example.com", "Suspended", "User", "Suspended123!@#")
        };

        foreach (var userInfo in users)
        {
            var userAccountId = UserAccountId.CreateNew().Value;
            var userName = UserName.Create(userInfo.userName).Value;
            var email = Email.Create(userInfo.email).Value;
            var personName = PersonName.Create(userInfo.firstName, userInfo.lastName).Value;

            var userAccount = UserAccount.Create(
                userAccountId,
                userName,
                email,
                UserStatus.Active,
                personName);

            var result = await _identityService.CreateUserAsync(userAccount, userInfo.password);
            _output.WriteLine($"Created user {userInfo.userName}: {result.IsSuccess}");
        }

        // Create custom roles with permissions
        var roles = new Dictionary<string, List<string>>
        {
            ["ContentEditor"] = new List<string> {
                "Content.Create", "Content.Edit", "Content.View", "Content.Delete"
            },
            ["ContentViewer"] = new List<string> {
                "Content.View"
            },
            ["UserManager"] = new List<string> {
                "Users.Create", "Users.Edit", "Users.Delete", "Users.View", "Users.Assign"
            },
            ["SystemAdmin"] = new List<string> {
                "System.Configure", "System.Backup", "System.Restore", "System.ViewLogs",
                "Users.Create", "Users.Edit", "Users.Delete", "Users.View", "Users.Assign",
                "Content.Create", "Content.Edit", "Content.View", "Content.Delete"
            },
            ["DeveloperRole"] = new List<string> {
                "System.Configure", "System.ViewLogs", "API.Access", "API.Create",
                "Content.Create", "Content.Edit", "Content.View"
            },
            ["ReadOnlyRole"] = new List<string> {
                "Content.View", "Users.View", "System.ViewLogs"
            }
        };

        foreach (var role in roles)
        {
            var result = await _identityService.CreateRoleAsync(role.Key, role.Value);
            _output.WriteLine($"Created role {role.Key}: {result.IsSuccess}");
        }

        // Assign roles to users
        var userRoles = new Dictionary<string, List<string>>
        {
            ["developer"] = new List<string> { "DeveloperRole" },
            ["manager"] = new List<string> { "UserManager", "ContentEditor" },
            ["admin"] = new List<string> { "SystemAdmin" },
            ["readonly"] = new List<string> { "ReadOnlyRole", "ContentViewer" },
            ["suspended"] = new List<string> { } // No roles for suspended user
        };

        foreach (var userRole in userRoles)
        {
            var user = await _fixture.UserManager.FindByNameAsync(userRole.Key);
            if (user != null)
            {
                var userAccountId = UserAccountId.Create(Guid.Parse(user.DomainId)).Value;

                foreach (var roleName in userRole.Value)
                {
                    var result = await _identityService.AddUserToRoleAsync(userAccountId, roleName);
                    _output.WriteLine($"Assigned role {roleName} to user {userRole.Key}: {result.IsSuccess}");
                }
            }
        }

        // Update some user properties
        var suspendedUser = await _fixture.UserManager.FindByNameAsync("suspended");
        if (suspendedUser != null)
        {
            // Map to domain entity
            var userAccount = _mapper.MapToUserAccount(suspendedUser);

            // Update status to inactive
            userAccount.UpdateStatus(UserStatus.Inactive);

            // Save back
            await _identityService.UpdateUserAsync(userAccount);
            _output.WriteLine("Updated suspended user status to Inactive");
        }

        // Add custom claims to a user
        var developerUser = await _fixture.UserManager.FindByNameAsync("developer");
        if (developerUser != null)
        {
            await _fixture.UserManager.AddClaimAsync(developerUser, new Claim("Department", "Engineering"));
            await _fixture.UserManager.AddClaimAsync(developerUser, new Claim("EmployeeId", "DEV-123"));
            await _fixture.UserManager.AddClaimAsync(developerUser, new Claim("AccessLevel", "Restricted"));
            _output.WriteLine("Added custom claims to developer user");
        }
    }

    [Fact]
    public async Task ExamineUserRolesAndPermissions()
    {
        // This test is for programmatically examining users, roles, and permissions
        _output.WriteLine("Examining Users, Roles and Permissions...");

        // Get all users
        var users = _fixture.UserManager.Users.ToList();
        _output.WriteLine($"Total Users: {users.Count}");

        foreach (var user in users)
        {
            _output.WriteLine($"\nUser: {user.UserName} ({user.Email})");

            // Get roles for this user
            var roles = await _fixture.UserManager.GetRolesAsync(user);
            _output.WriteLine($"Roles: {string.Join(", ", roles)}");

            // Get all permissions via roles
            var permissions = new HashSet<string>();
            foreach (var roleName in roles)
            {
                var role = await _fixture.RoleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    var claims = await _fixture.RoleManager.GetClaimsAsync(role);
                    foreach (var claim in claims.Where(c => c.Type == "Permission"))
                    {
                        permissions.Add(claim.Value);
                    }
                }
            }

            _output.WriteLine($"Permissions via roles: {string.Join(", ", permissions)}");

            // Get direct claims
            var userClaims = await _fixture.UserManager.GetClaimsAsync(user);
            if (userClaims.Any())
            {
                _output.WriteLine("Direct claims:");
                foreach (var claim in userClaims)
                {
                    _output.WriteLine($"  {claim.Type}: {claim.Value}");
                }
            }
        }

        _output.WriteLine("\nDatabase examination complete");
    }
}