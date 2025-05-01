using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Base;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using Moq;

using System.Security.Claims;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Services;

/// <summary>
/// Unit tests for the <see cref="EFCoreCurrentUserService"/> class.
/// </summary>
public class EFCoreCurrentUserServiceTests : EFCoreIdentityTestBase
{
    /// <summary>
    /// Creates a mock <see cref="HttpContext"/> with the specified <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <param name="principal">The claims principal to set in the mock HTTP context.</param>
    /// <returns>A mock <see cref="HttpContext"/> instance.</returns>
    private HttpContext CreateMockHttpContext(ClaimsPrincipal principal)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.User = principal;
        return httpContext;
    }

    /// <summary>
    /// Tests the <see cref="EFCoreCurrentUserService.IsInRoleAsync(string, CancellationToken)"/> method.
    /// Verifies that it returns <c>true</c> when the user is in the specified role.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task IsInRoleAsync_UserInRole_ReturnsTrue()
    {
        // Arrange
        var (userManager, roleManager, dbContext) = CreateUserAndRoleManagers();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<EFCoreCurrentUserService>>().Object;

        // Create user and role
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER", // Set normalized values
            NormalizedEmail = "TEST@EXAMPLE.COM"
        };

        // Create and add roles directly to the context to avoid using AddToRoleAsync
        var role = new ApplicationRole("TestRole")
        {
            Id = Guid.NewGuid().ToString(),
            NormalizedName = "TESTROLE"
        };

        await userManager.CreateAsync(user, "Password123!").ConfigureAwait(true);
        await roleManager.CreateAsync(role).ConfigureAwait(true);

        // Create and add the user-role relationship directly
        var userRole = new ApplicationUserRole(user.Id, role.Id);
        dbContext.Set<ApplicationUserRole>().Add(userRole);
        await dbContext.SaveChangesAsync().ConfigureAwait(true);

        // Create claims principal with user ID claim
        var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, user.Id),
        new(ClaimTypes.Name, user.UserName),
        new(ClaimTypes.Role, "TestRole")
    };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(CreateMockHttpContext(principal));

        var service = new EFCoreCurrentUserService(
            httpContextAccessor.Object,
            userManager,
            roleManager,
            dbContext,
            logger);

        // Act
        var result = await service.IsInRoleAsync("TestRole").ConfigureAwait(true);

        // Assert
        Assert.True(result);
    }

    /// <summary>
    /// Tests the <see cref="EFCoreCurrentUserService.UserId"/> property.
    /// Verifies that it returns the correct domain ID when the user is authenticated and has a "DomainId" claim.
    /// </summary>
    [Fact]
    public void UserId_AuthenticatedUserWithDomainIdClaim_ReturnsCorrectId()
    {
        // Arrange
        var (userManager, roleManager, dbContext) = CreateUserAndRoleManagers();
        var httpContextAccessor = new Mock<IHttpContextAccessor>();
        var logger = new Mock<ILogger<EFCoreCurrentUserService>>().Object;

        var domainId = Guid.NewGuid();

        // Create claims principal with domain ID claim
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim("DomainId", domainId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        httpContextAccessor.Setup(x => x.HttpContext)
            .Returns(CreateMockHttpContext(principal));

        var service = new EFCoreCurrentUserService(
            httpContextAccessor.Object,
            userManager,
            roleManager,
            dbContext,
            logger);

        // Act
        var userId = service.UserId;

        // Assert
        Assert.NotNull(userId);
        Assert.Equal(domainId, userId.Value);
    }

    // Additional tests for other methods
}
