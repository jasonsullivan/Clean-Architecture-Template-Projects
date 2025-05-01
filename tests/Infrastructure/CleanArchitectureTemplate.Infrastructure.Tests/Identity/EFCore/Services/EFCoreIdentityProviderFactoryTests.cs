using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Mapping;
using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Services;
using CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Base;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Moq;

namespace CleanArchitectureTemplate.Infrastructure.Tests.Identity.EFCore.Services;

public class EFCoreIdentityProviderFactoryTests : EFCoreIdentityTestBase
{
    private (EFCoreIdentityService, EFCoreCurrentUserService) CreateServices()
    {
        var (userManager, roleManager, dbContext) = CreateUserAndRoleManagers();
        var httpContextAccessor = new Mock<IHttpContextAccessor>().Object;
        var currentUserLogger = new Mock<ILogger<EFCoreCurrentUserService>>().Object;
        var identityServiceLogger = new Mock<ILogger<EFCoreIdentityService>>().Object;
        var mapperLogger = new Mock<ILogger<IdentityMapper>>().Object;

        var mapper = new IdentityMapper(mapperLogger);

        var currentUserService = new EFCoreCurrentUserService(
            httpContextAccessor,
            userManager,
            roleManager,
            dbContext,
            currentUserLogger);

        var identityService = new EFCoreIdentityService(
            userManager,
            roleManager,
            dbContext,
            mapper,
            identityServiceLogger);

        return (identityService, currentUserService);
    }

    [Fact]
    public void CreateCurrentUserService_ReturnsEFCoreCurrentUserService()
    {
        // Arrange
        var (identityService, currentUserService) = CreateServices();
        var options = Options.Create(new EFCoreIdentityOptions());
        var logger = new Mock<ILogger<EFCoreIdentityProviderFactory>>().Object;

        var factory = new EFCoreIdentityProviderFactory(
            identityService,
            currentUserService,
            options,
            logger);

        // Act
        var result = factory.CreateCurrentUserService();

        // Assert
        Assert.Same(currentUserService, result);
    }

    [Fact]
    public void CreateIdentityService_ReturnsEFCoreIdentityService()
    {
        // Arrange
        var (identityService, currentUserService) = CreateServices();
        var options = Options.Create(new EFCoreIdentityOptions());
        var logger = new Mock<ILogger<EFCoreIdentityProviderFactory>>().Object;

        var factory = new EFCoreIdentityProviderFactory(
            identityService,
            currentUserService,
            options,
            logger);

        // Act
        var result = factory.CreateIdentityService();

        // Assert
        Assert.Same(identityService, result);
    }

    [Fact]
    public void GetActiveProviderName_ReturnsEFCoreIdentity()
    {
        // Arrange
        var (identityService, currentUserService) = CreateServices();
        var options = Options.Create(new EFCoreIdentityOptions());
        var logger = new Mock<ILogger<EFCoreIdentityProviderFactory>>().Object;

        var factory = new EFCoreIdentityProviderFactory(
            identityService,
            currentUserService,
            options,
            logger);

        // Act
        var providerName = factory.GetActiveProviderName();

        // Assert
        Assert.Equal("EFCoreIdentity", providerName);
    }
}