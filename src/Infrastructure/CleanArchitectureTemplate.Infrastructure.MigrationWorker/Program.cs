using CleanArchitectureTemplate.Infrastructure.Identity.EFCore.Persistence;

using Microsoft.EntityFrameworkCore;

namespace CleanArchitectureTemplate.Infrastructure.MigrationWorker;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.AddServiceDefaults();

        builder.Services.AddDbContextPool<ApplicationIdentityDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("cleanarchitecturetemplate"),
                sqlOptions => sqlOptions.MigrationsAssembly("CleanArchitectureTemplate.Infrastructure.MigrationWorker")
        ));

        builder.EnrichSqlServerDbContext<ApplicationIdentityDbContext>();

        Console.WriteLine($"Connection String: {builder.Configuration.GetConnectionString("cleanarchitecturetemplate")}");

        builder.Services.AddOpenTelemetry()
            .WithTracing(tracing => tracing.AddSource(DatabaseInitializerService.ActivitySourceName));

        builder.Services.AddSingleton<DatabaseInitializerService>();

        builder.Services.AddHostedService(sp => sp.GetRequiredService<DatabaseInitializerService>());

        builder.Services.AddHealthChecks().AddCheck<ApplicationDbInitializerHealthCheck>("DbInitializer", null);

        var host = builder.Build();

        host.Run();
    }
}
