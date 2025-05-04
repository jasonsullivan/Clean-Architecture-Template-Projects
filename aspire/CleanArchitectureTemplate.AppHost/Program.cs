
public class Program
{
    public static void Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        var password = builder.AddParameter("password", secret: true, value: "P@ssw0rd");

        builder.AddProject<Projects.CleanArchitectureTemplate_Infrastructure_MigrationWorker>("cleanarchitecturetemplate-infrastructure-migrationworker");

        builder.AddProject<Projects.CleanArchitectureTemplate_API>("cleanarchitecturetemplate-api");

        builder.AddProject<Projects.CleanArchitectureTemplate_Web>("cleanarchitecturetemplate-web");

        builder.Build().Run();
    }
}

