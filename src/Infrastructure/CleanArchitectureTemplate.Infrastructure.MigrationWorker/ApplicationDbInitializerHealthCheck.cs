using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace CleanArchitectureTemplate.Infrastructure.MigrationWorker;

/// <summary>
/// A health check to monitor the status of the database initialization process.
/// </summary>
/// <param name="databaseInitializerService">The database initializer service to monitor.</param>
internal class ApplicationDbInitializerHealthCheck(DatabaseInitializerService databaseInitializerService) : IHealthCheck
{
    /// <summary>
    /// Checks the health of the database initialization process.
    /// </summary>
    /// <param name="context">The health check context.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the health check result.</returns>
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var task = databaseInitializerService.ExecuteTask;

        return task switch
        {
            { IsCompletedSuccessfully: true } => Task.FromResult(HealthCheckResult.Healthy()),
            { IsFaulted: true } => Task.FromResult(HealthCheckResult.Unhealthy(task.Exception?.InnerException?.Message, task.Exception)),
            { IsCanceled: true } => Task.FromResult(HealthCheckResult.Unhealthy("Database initialization was canceled")),
            _ => Task.FromResult(HealthCheckResult.Degraded("Database initialization is still in progress"))
        };
    }
}