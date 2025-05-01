using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Base;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Services;

/// <summary>
/// Unit tests for the <see cref="EFCoreIdentityService"/> class.
/// </summary>
public class EFCoreIdentityServiceTests : EFCoreIdentityTestBase
{
    /// <summary>
    /// Tests that <see cref="EFCoreIdentityService.CreateUserAsync"/> creates a user with valid data
    /// and returns the correct user ID.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task CreateUserAsync_WithValidData_CreatesUserAndReturnsCorrectId()
    {
        // Arrange
        var (userManager, roleManager, dbContext) = CreateUserAndRoleManagers();
        var logger = new Mock<ILogger<EFCoreIdentityService>>().Object;
        var mapperLogger = new Mock<ILogger<IdentityMapper>>().Object;
        var mapper = new IdentityMapper(mapperLogger);

        var service = new EFCoreIdentityService(
            userManager,
            roleManager,
            dbContext,
            mapper,
            logger);

        var userAccountId = UserAccountId.CreateNew().Value;
        var userName = UserName.Create("testuser").Value;
        var email = Email.Create("test@example.com").Value;
        var status = UserStatus.Active;
        var personName = PersonName.Create("Test", "User").Value;

        var userAccount = UserAccount.Create(
            userAccountId,
            userName,
            email,
            status,
            personName);

        // Act
        var result = await service.CreateUserAsync(userAccount, "Password123!").ConfigureAwait(true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(userAccountId.Value, result.Value.Value);

        var createdUser = await userManager.FindByNameAsync("testuser").ConfigureAwait(true);
        Assert.NotNull(createdUser);
        Assert.Equal(userAccount.Email.Value, createdUser.Email);
        Assert.Equal(userAccount.Id.Value.ToString(), createdUser.DomainId);
    }

    /// <summary>
    /// Tests that <see cref="EFCoreIdentityService.GetUserByIdAsync"/> retrieves the correct user
    /// when provided with an existing user ID.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ReturnsCorrectUser()
    {
        // Arrange
        var (userManager, roleManager, dbContext) = CreateUserAndRoleManagers();
        var logger = new Mock<ILogger<EFCoreIdentityService>>().Object;
        var mapperLogger = new Mock<ILogger<IdentityMapper>>().Object;
        var mapper = new IdentityMapper(mapperLogger);

        var service = new EFCoreIdentityService(
            userManager,
            roleManager,
            dbContext,
            mapper,
            logger);

        // Create a test user
        var userId = Guid.NewGuid();
        var domainId = userId.ToString();
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(), // Different from domain ID
            UserName = "testuser",
            Email = "test@example.com",
            NormalizedUserName = "TESTUSER",
            NormalizedEmail = "TEST@EXAMPLE.COM",
            EmailConfirmed = true,
            FirstName = "Test",
            LastName = "User",
            DomainId = domainId
        };

        await userManager.CreateAsync(user, "Password123!").ConfigureAwait(true);

        // Act
        var result = await service.GetUserByIdAsync(UserAccountId.Create(userId).Value).ConfigureAwait(true);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("testuser", result.Value.Username.Value);
        Assert.Equal("test@example.com", result.Value.Email.Value);
        Assert.Equal("Test", result.Value.PersonName.FirstName);
        Assert.Equal("User", result.Value.PersonName.LastName);
        Assert.Equal(userId, result.Value.Id.Value);
    }

    // Additional tests for UpdateUserAsync, DeleteUserAsync, etc.
}
