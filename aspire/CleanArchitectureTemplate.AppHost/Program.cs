var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.CleanArchitectureTemplate_Infrastructure_MigrationWorker>("cleanarchitecturetemplate-infrastructure-migrationworker");

builder.AddProject<Projects.CleanArchitectureTemplate_API>("cleanarchitecturetemplate-api");

builder.AddProject<Projects.CleanArchitectureTemplate_Web>("cleanarchitecturetemplate-web");

builder.Build().Run();
