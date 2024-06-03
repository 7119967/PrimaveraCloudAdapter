namespace PCA.Configurations.Application;

public static class DbConnectionHelper
{
    public static string? GetDbConnection(IConfiguration configuration, IHostEnvironment env)
    {
        var connectionString = env.IsDevelopment()
            ? configuration.GetConnectionString("LocalConnection")
            : configuration.GetConnectionString("ServerConnection");

        var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
        var logger = loggerFactory.CreateLogger("ConnectionStringLogger");
        logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

        return connectionString;
    }
}
