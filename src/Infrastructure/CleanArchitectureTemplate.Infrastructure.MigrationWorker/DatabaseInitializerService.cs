using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

using System.Diagnostics;

namespace CleanArchitectureTemplate.Infrastructure.MigrationWorker;

/// <summary>
/// A background service responsible for initializing the database and applying migrations.
/// </summary>
/// <param name="serviceProvider">The service provider to resolve dependencies.</param>
/// <param name="hostEnvironment">The host environment of the application.</param>
/// <param name="hostApplicationLifetime">The application lifetime to manage application shutdown.</param>
public class DatabaseInitializerService(
    IServiceProvider serviceProvider,
    IHostEnvironment hostEnvironment,
    IHostApplicationLifetime hostApplicationLifetime) : BackgroundService
{
    private readonly ActivitySource _activitySource = new(hostEnvironment.ApplicationName);

    public const string ActivitySourceName = "DatabaseInitializerService - Migrations";

    /// <summary>
    /// Executes the database initialization and migration tasks.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var activity = _activitySource.StartActivity(hostEnvironment.ApplicationName, ActivityKind.Client);

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationIdentityDbContext>();

            await EnsureDatabaseAsync(dbContext, cancellationToken).ConfigureAwait(true);
            await RunMigrationAsync(dbContext, cancellationToken).ConfigureAwait(true);
        }
        catch (Exception ex)
        {
            activity?.AddException(ex);
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    /// <summary>
    /// Ensures that the database exists, creating it if necessary.
    /// </summary>
    /// <param name="dbContext">The application's database context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    private static async Task EnsureDatabaseAsync(ApplicationIdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var dbCreator = dbContext.GetService<IRelationalDatabaseCreator>();

        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it does not exist.
            if (!await dbCreator.ExistsAsync(cancellationToken).ConfigureAwait(true))
            {
                await dbCreator.CreateAsync(cancellationToken).ConfigureAwait(true);
            }
        }).ConfigureAwait(true);
    }

    /// <summary>
    /// Applies pending migrations to the database within a transaction.
    /// </summary>
    /// <param name="dbContext">The application's database context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    private static async Task RunMigrationAsync(ApplicationIdentityDbContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            // Run migration in a transaction to avoid partial migration if it fails.
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(true);
            await dbContext.Database.MigrateAsync(cancellationToken).ConfigureAwait(true);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(true);
        }).ConfigureAwait(true);
    }
}
