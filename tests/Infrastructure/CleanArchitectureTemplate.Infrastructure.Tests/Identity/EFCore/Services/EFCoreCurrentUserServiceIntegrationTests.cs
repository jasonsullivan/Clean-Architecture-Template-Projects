using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using System.Security.Claims;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Services;

public class EFCoreCurrentUserServiceIntegrationTests : IClassFixture<EFCoreIdentityIntegrationTestFixture>
{
    private readonly EFCoreIdentityIntegrationTestFixture _fixture;
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly ILogger<EFCoreCurrentUserService> _logger;

    public EFCoreCurrentUserServiceIntegrationTests(EFCoreIdentityIntegrationTestFixture fixture)
    {
        _fixture = fixture;

        // Create a mock HTTP context accessor
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();

        // Create a mock logger for the current user service
        var loggerMock = new Mock<ILogger<EFCoreCurrentUserService>>();
        _logger = loggerMock.Object;

        // Seed test data using the fixture's UserManager and RoleManager
        IdentityTestDataSeeder.SeedBasicUserAsync(_fixture.UserManager, _fixture.RoleManager).Wait();
        IdentityTestDataSeeder.SeedRolesWithPermissionsAsync(_fixture.RoleManager).Wait();
    }

    private ClaimsPrincipal CreateClaimsPrincipal(ApplicationUser user, string[] roles = null)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim("DomainId", user.DomainId)
        };

        // Add role claims
        if (roles != null)
        {
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }

        var identity = new ClaimsIdentity(claims, "Test");
        return new ClaimsPrincipal(identity);
    }

    private void SetCurrentUser(ApplicationUser user, string[] roles = null)
    {
        var principal = CreateClaimsPrincipal(user, roles);
        var httpContext = new DefaultHttpContext { User = principal };
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);
    }

    [Fact]
    public async Task IsInRoleAsync_UserInRole_ReturnsTrue()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("admin");
        Assert.NotNull(existingUser);

        SetCurrentUser(existingUser, new[] { "Administrator" });

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = await currentUserService.IsInRoleAsync("Administrator");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsInRoleAsync_UserNotInRole_ReturnsFalse()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.NotNull(existingUser);

        SetCurrentUser(existingUser, new[] { "User" });

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = await currentUserService.IsInRoleAsync("Administrator");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsCorrectRoles()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("admin");
        Assert.NotNull(existingUser);

        SetCurrentUser(existingUser, new[] { "Administrator" });

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = await currentUserService.GetRolesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Administrator", result.Value);
    }

    [Fact]
    public async Task HasPermissionAsync_UserHasPermission_ReturnsTrue()
    {
        // Arrange
        // Assign a permission to the Administrator role
        var adminRole = await _fixture.RoleManager.FindByNameAsync("Administrator");
        Assert.NotNull(adminRole);

        await _fixture.RoleManager.AddClaimAsync(adminRole, new Claim("Permission", "Users.Manage"));

        var existingUser = await _fixture.UserManager.FindByNameAsync("admin");
        Assert.NotNull(existingUser);

        SetCurrentUser(existingUser, new[] { "Administrator" });

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = await currentUserService.HasPermissionAsync("Users.Manage");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void UserId_WithAuthenticatedUser_ReturnsCorrectId()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var domainId = userId.ToString();

        var userMock = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "test@example.com",
            DomainId = domainId
        };

        SetCurrentUser(userMock);

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = currentUserService.UserId;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.Value);
    }

    [Fact]
    public void IsAuthenticated_WithAuthenticatedUser_ReturnsTrue()
    {
        // Arrange
        var userMock = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "test@example.com",
            DomainId = Guid.NewGuid().ToString()
        };

        SetCurrentUser(userMock);

        var currentUserService = new EFCoreCurrentUserService(
            _httpContextAccessorMock.Object,
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _logger);

        // Act
        var result = currentUserService.IsAuthenticated;

        // Assert
        Assert.True(result);
    }
}