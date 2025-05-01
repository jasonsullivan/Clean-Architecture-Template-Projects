using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Services;

public class EFCoreIdentityServiceIntegrationTests : IClassFixture<EFCoreIdentityIntegrationTestFixture>
{
    private readonly EFCoreIdentityIntegrationTestFixture _fixture;
    private readonly EFCoreIdentityService _identityService;
    private readonly IdentityMapper _mapper;

    public EFCoreIdentityServiceIntegrationTests(EFCoreIdentityIntegrationTestFixture fixture)
    {
        _fixture = fixture;

        // Create the mapper
        var loggerMock = new Mock<ILogger<IdentityMapper>>();
        _mapper = new IdentityMapper(loggerMock.Object);

        // Create the service under test
        var serviceMock = new Mock<ILogger<EFCoreIdentityService>>();
        _identityService = new EFCoreIdentityService(
            _fixture.UserManager,
            _fixture.RoleManager,
            _fixture.DbContext,
            _mapper,
            serviceMock.Object);

        // Seed test data
        IdentityTestDataSeeder.SeedBasicUserAsync(_fixture.UserManager, _fixture.RoleManager).Wait();
        IdentityTestDataSeeder.SeedRolesWithPermissionsAsync(_fixture.RoleManager).Wait();
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_CreatesUserAndReturnsId()
    {
        // Arrange
        var userAccountId = UserAccountId.CreateNew().Value;
        var userName = UserName.Create("newTestUser").Value;
        var email = Email.Create("newtest@example.com").Value;
        var personName = PersonName.Create("New", "User").Value;

        // Ensure no conflicting data exists in the database
        var existingUser = await _fixture.UserManager.FindByNameAsync("newTestUser");
        if (existingUser != null)
        {
            await _fixture.UserManager.DeleteAsync(existingUser);
        }

        var existingEmail = await _fixture.UserManager.FindByEmailAsync("newtest@example.com");
        if (existingEmail != null)
        {
            await _fixture.UserManager.DeleteAsync(existingEmail);
        }

        var userAccount = UserAccount.Create(
            userAccountId,
            userName,
            email,
            UserStatus.Active,
            personName);

        // Act
        var result = await _identityService.CreateUserAsync(userAccount, "Password123!");

        // Assert
        Assert.True(result.IsSuccess, "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        Assert.Equal(userAccountId.Value, result.Value.Value);

        // Verify user was created in the database
        var createdUser = await _fixture.UserManager.FindByNameAsync("newTestUser");
        Assert.NotNull(createdUser);
        Assert.Equal(userAccount.Email.Value, createdUser.Email);
        Assert.Equal(userAccount.Id.Value.ToString(), createdUser.DomainId);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithExistingUser_ReturnsCorrectUser()
    {
        // Arrange
        // Find a user that we know exists from our seed data
        var existingUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.NotNull(existingUser); // Verify seed data is present

        var userAccountId = UserAccountId.Create(Guid.Parse(existingUser.DomainId)).Value;

        // Act
        var result = await _identityService.GetUserByIdAsync(userAccountId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("testuser", result.Value.Username.Value);
        Assert.Equal("test@example.com", result.Value.Email.Value);
        Assert.Equal("Test", result.Value.PersonName.FirstName);
        Assert.Equal("User", result.Value.PersonName.LastName);
    }

    [Fact]
    public async Task AddUserToRoleAsync_WithValidData_AssignsRoleToUser()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.NotNull(existingUser);

        var userAccountId = UserAccountId.Create(Guid.Parse(existingUser.DomainId)).Value;

        // Make sure the role exists (in case the role seeding didn't complete properly)
        var roleExists = await _fixture.RoleManager.RoleExistsAsync("ContentEditor");
        if (!roleExists)
        {
            var role = new ApplicationRole("ContentEditor");
            var createRoleResult = await _fixture.RoleManager.CreateAsync(role);
            Assert.True(createRoleResult.Succeeded, "Failed to create ContentEditor role");
        }

        // Act
        var result = await _identityService.AddUserToRoleAsync(userAccountId, "ContentEditor");

        // Assert
        if (!result.IsSuccess)
        {
            // If the test fails, output the error for debugging
            Console.WriteLine($"Error: {string.Join(", ", result.Errors.Select(e => $"{e.Code}: {e.Description}"))}");
        }

        Assert.True(result.IsSuccess);

        // Verify role was assigned
        var userRoles = await _fixture.UserManager.GetRolesAsync(existingUser);
        Assert.Contains("ContentEditor", userRoles);
    }

    [Fact]
    public async Task GetUserRolesAsync_WithExistingUser_ReturnsCorrectRoles()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("admin");
        Assert.NotNull(existingUser);

        var userAccountId = UserAccountId.Create(Guid.Parse(existingUser.DomainId)).Value;

        // Act
        var result = await _identityService.GetUserRolesAsync(userAccountId);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Administrator", result.Value);
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_UpdatesUserInDatabase()
    {
        // Arrange
        var existingUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.NotNull(existingUser);

        var userAccountId = UserAccountId.Create(Guid.Parse(existingUser.DomainId)).Value;
        var userResult = await _identityService.GetUserByIdAsync(userAccountId);
        Assert.True(userResult.IsSuccess);

        var userAccount = userResult.Value;

        // Update user properties
        userAccount.UpdatePersonName(PersonName.Create("Updated", "User").Value);

        // Act
        var result = await _identityService.UpdateUserAsync(userAccount);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify user was updated in the database
        var updatedUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.Equal("Updated", updatedUser.FirstName);
        Assert.Equal("User", updatedUser.LastName);
    }

    [Fact]
    public async Task DeleteUserAsync_WithExistingUser_RemovesUserFromDatabase()
    {
        // Arrange
        // Create a user to delete
        var userToDelete = new ApplicationUser
        {
            UserName = "userToDelete",
            Email = "delete@example.com",
            EmailConfirmed = true,
            FirstName = "Delete",
            LastName = "User",
            DomainId = Guid.NewGuid().ToString()
        };

        await _fixture.UserManager.CreateAsync(userToDelete, "Password123!");

        var userAccountId = UserAccountId.Create(Guid.Parse(userToDelete.DomainId)).Value;

        // Act
        var result = await _identityService.DeleteUserAsync(userAccountId);

        // Assert
        Assert.True(result.IsSuccess);

        // Verify user was deleted from the database
        var deletedUser = await _fixture.UserManager.FindByNameAsync("userToDelete");
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task GetRolesAsync_ReturnsAllRoles()
    {
        // Act
        var result = await _identityService.GetRolesAsync();

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Administrator", result.Value);
        Assert.Contains("User", result.Value);
        Assert.Contains("ContentEditor", result.Value);
        Assert.Contains("ContentViewer", result.Value);
        Assert.Contains("UserManager", result.Value);
    }
}