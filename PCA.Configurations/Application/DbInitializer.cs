namespace PCA.Configurations.Application;

public static class DbInitializer
{
    public static void InitializeDatabase(IServiceCollection services, ILogger logger)
    {
        var serviceProvider = services.BuildServiceProvider();
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            dbContext.Database.OpenConnection();
        }
        catch (Exception ex)
        {
            throw new BaseException($"An error occurred while connecting to the {dbContext.Database.ProviderName}: {ex.Message}");
        }

        if (dbContext.Database.CanConnect())
        {
            logger?.LogInformation($"There is a connection to the database: {dbContext.Database.ProviderName}\nMigrations started");
            dbContext.Database.Migrate();
            logger?.LogInformation("Migrations completed");
        }
        else
        {
            logger?.LogError($"There is no connection to the database: {dbContext.Database.ProviderName}");
        }
    }
}
