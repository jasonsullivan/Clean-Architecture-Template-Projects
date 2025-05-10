using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

/// <summary>
/// Database context for ASP.NET Core Identity, using custom application entities.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ApplicationIdentityDbContext"/> class.
/// </remarks>
/// <param name="options">The options to be used by the DbContext.</param>
public sealed class ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options) : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    string,
    ApplicationUserClaim,
    ApplicationUserRole,
    ApplicationUserLogin,
    IdentityRoleClaim<string>,
    IdentityUserToken<string>>(options)
{

    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="builder">The builder being used to construct the model for this context.</param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure custom Identity tables if needed
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users");

            // Configure the DomainId property
            entity.Property(e => e.DomainId)
                  .HasMaxLength(36) // GUID string length
                  .IsUnicode(false);

            // Add an index on DomainId for faster lookups
            entity.HasIndex(e => e.DomainId)
                  .HasDatabaseName("IX_Users_DomainId")
                  .IsUnique();

            // Configure the IdentityProvider property
            entity.Property(e => e.IdentityProvider)
                  .HasMaxLength(50)
                  .IsUnicode(false);

            entity.Property(e => e.PasswordChangeRequired)
                  .HasDefaultValue(false);
        });

        builder.Entity<ApplicationRole>(entity =>
        {
            entity.ToTable("Roles");
        });

        builder.Entity<ApplicationUserRole>(entity =>
        {
            entity.ToTable("UserRoles");
        });

        builder.Entity<ApplicationUserClaim>(entity =>
        {
            entity.ToTable("UserClaims");
        });

        builder.Entity<ApplicationUserLogin>(entity =>
        {
            entity.ToTable("UserLogins");
        });

        builder.Entity<IdentityRoleClaim<string>>(entity =>
        {
            entity.ToTable("RoleClaims");
        });

        builder.Entity<IdentityUserToken<string>>(entity =>
        {
            entity.ToTable("UserTokens");
        });
    }
}
