using CleanArchitectureTemplate.Domain.UserAccounts.Entities;
using CleanArchitectureTemplate.Domain.UserAccounts.ValueObjects;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Base;

using Microsoft.Extensions.Logging;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Mapping;

/// <summary>
/// Unit tests for the <see cref="IdentityMapper"/> class.
/// </summary>
public class IdentityMapperTests : EFCoreIdentityTestBase
{
    /// <summary>
    /// Tests that <see cref="IdentityMapper.MapToUserAccount"/> correctly maps a valid <see cref="ApplicationUser"/> to a <see cref="UserAccount"/>.
    /// </summary>
    [Fact]
    public void MapToUserAccount_WithValidApplicationUser_ReturnsCorrectUserAccount()
    {
        // Arrange
        var logger = new Mock<ILogger<IdentityMapper>>().Object;
        var mapper = new IdentityMapper(logger);

        var applicationUser = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            DomainId = Guid.NewGuid().ToString()
        };

        // Act
        var userAccount = mapper.MapToUserAccount(applicationUser);

        // Assert
        Assert.NotNull(userAccount);
        Assert.Equal(applicationUser.UserName, userAccount.Username.Value);
        Assert.Equal(applicationUser.Email, userAccount.Email.Value);
        Assert.Equal(applicationUser.FirstName, userAccount.PersonName.FirstName);
        Assert.Equal(applicationUser.LastName, userAccount.PersonName.LastName);
        Assert.Equal(applicationUser.DomainId, userAccount.Id.Value.ToString());
    }

    /// <summary>
    /// Tests that <see cref="IdentityMapper.MapToApplicationUser"/> correctly maps a valid <see cref="UserAccount"/> to an <see cref="ApplicationUser"/>.
    /// </summary>
    [Fact]
    public void MapToApplicationUser_WithValidUserAccount_ReturnsCorrectApplicationUser()
    {
        // Arrange
        var logger = new Mock<ILogger<IdentityMapper>>().Object;
        var mapper = new IdentityMapper(logger);

        var userAccountId = UserAccountId.Create(Guid.NewGuid()).Value;
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
        var applicationUser = mapper.MapToApplicationUser(userAccount);

        // Assert
        Assert.NotNull(applicationUser);
        Assert.Equal(userAccount.Username.Value, applicationUser.UserName);
        Assert.Equal(userAccount.Email.Value, applicationUser.Email);
        Assert.Equal(userAccount.PersonName.FirstName, applicationUser.FirstName);
        Assert.Equal(userAccount.PersonName.LastName, applicationUser.LastName);
        Assert.Equal(userAccount.Id.Value.ToString(), applicationUser.DomainId);
    }
}