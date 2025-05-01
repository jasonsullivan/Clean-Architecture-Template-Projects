using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.TestFixtures;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Mapping;

public class IdentityMapperIntegrationTests : IClassFixture<EFCoreIdentityIntegrationTestFixture>
{
    private readonly EFCoreIdentityIntegrationTestFixture _fixture;
    private readonly IdentityMapper _mapper;

    public IdentityMapperIntegrationTests(EFCoreIdentityIntegrationTestFixture fixture)
    {
        _fixture = fixture;

        // Create the mapper
        var loggerMock = new Mock<ILogger<IdentityMapper>>();
        _mapper = new IdentityMapper(loggerMock.Object);

        // Seed basic test data
        IdentityTestDataSeeder.SeedBasicUserAsync(_fixture.UserManager, _fixture.RoleManager).Wait();
    }

    [Fact]
    public async Task MapToUserAccount_WithDatabaseEntity_MapsCorrectly()
    {
        // Arrange
        var dbUser = await _fixture.UserManager.FindByNameAsync("testuser");
        Assert.NotNull(dbUser); // Verify seed data is present

        // Act
        var userAccount = _mapper.MapToUserAccount(dbUser);

        // Assert
        Assert.NotNull(userAccount);
        Assert.Equal(dbUser.UserName, userAccount.Username.Value);
        Assert.Equal(dbUser.Email, userAccount.Email.Value);
        Assert.Equal(dbUser.FirstName, userAccount.PersonName.FirstName);
        Assert.Equal(dbUser.LastName, userAccount.PersonName.LastName);
        Assert.Equal(dbUser.DomainId, userAccount.Id.Value.ToString());
    }

    [Fact]
    public async Task MapToApplicationUser_ThenSaveToDatabase_WorksCorrectly()
    {
        // Arrange
        var userAccountId = UserAccountId.CreateNew().Value;
        var userName = UserName.Create("mappedUser").Value;
        var email = Email.Create("mapped@example.com").Value;
        var personName = PersonName.Create("Mapped", "User").Value;

        var userAccount = UserAccount.Create(
            userAccountId,
            userName,
            email,
            UserStatus.Active,
            personName);

        // Act
        var applicationUser = _mapper.MapToApplicationUser(userAccount);
        var result = await _fixture.UserManager.CreateAsync(applicationUser, "Password123!");

        // Assert
        Assert.True(result.Succeeded);

        // Verify user was created with correct properties
        var dbUser = await _fixture.UserManager.FindByNameAsync("mappedUser");
        Assert.NotNull(dbUser);
        Assert.Equal(userAccount.Email.Value, dbUser.Email);
        Assert.Equal(userAccount.Username.Value, dbUser.UserName);
        Assert.Equal(userAccount.PersonName.FirstName, dbUser.FirstName);
        Assert.Equal(userAccount.PersonName.LastName, dbUser.LastName);
        Assert.Equal(userAccount.Id.Value.ToString(), dbUser.DomainId);
    }

    [Fact]
    public async Task MapBidirectional_PreservesAllProperties()
    {
        // Arrange - Create a domain entity
        var userAccountId = UserAccountId.CreateNew().Value;
        var userName = UserName.Create("bidirectional").Value;
        var email = Email.Create("bidirectional@example.com").Value;
        var personName = PersonName.Create("Bidirectional", "Test", "Middle").Value;
        var phoneNumber = PhoneNumber.Create("+12345678901").Value;

        var originalUserAccount = UserAccount.Create(
            userAccountId,
            userName,
            email,
            UserStatus.Active,
            personName,
            phoneNumber);

        // Act - Map to ApplicationUser and back to UserAccount
        var applicationUser = _mapper.MapToApplicationUser(originalUserAccount);
        var mappedBackUserAccount = _mapper.MapToUserAccount(applicationUser);

        // Assert
        Assert.NotNull(mappedBackUserAccount);
        Assert.Equal(originalUserAccount.Id.Value, mappedBackUserAccount.Id.Value);
        Assert.Equal(originalUserAccount.Username.Value, mappedBackUserAccount.Username.Value);
        Assert.Equal(originalUserAccount.Email.Value, mappedBackUserAccount.Email.Value);
        Assert.Equal(originalUserAccount.PersonName.FirstName, mappedBackUserAccount.PersonName.FirstName);
        Assert.Equal(originalUserAccount.PersonName.LastName, mappedBackUserAccount.PersonName.LastName);
        Assert.Equal(originalUserAccount.PersonName.MiddleName, mappedBackUserAccount.PersonName.MiddleName);
        Assert.Equal(originalUserAccount.PhoneNumber.Value, mappedBackUserAccount.PhoneNumber.Value);
        Assert.Equal(originalUserAccount.Status, mappedBackUserAccount.Status);
    }
}