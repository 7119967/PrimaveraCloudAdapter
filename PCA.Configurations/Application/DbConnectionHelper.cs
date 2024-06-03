namespace PCA.Configurations.Application;

public static class DbConnectionHelper
{
    public static string? GetDbConnection(IConfiguration configuration, IHostEnvironment env)
    {
        var connectionString = env.IsDevelopment()
            ? configuration.GetConnectionString("LocalConnection")
            : configuration.GetConnectionString("ServerConnection");

        return connectionString;
    }
}
