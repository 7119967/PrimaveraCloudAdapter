using PCA.Infrastructure.Services.HttpClients;
using PCA.Infrastructure.Services.WebSocket;

namespace PCA.Configurations.DI;

public static class ServiceCollectionExtensions
{
    public static void ConfigureBusinessServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var scope = serviceProvider.GetService<IServiceScopeFactory>()!.CreateScope();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var logger = serviceProvider.GetService<ILogger>();
        // var scope = serviceProvider.CreateScope();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;

        //CultureInfo culture = new CultureInfo("ru-RU");
        //Thread.CurrentThread.CurrentCulture = culture;
        //Thread.CurrentThread.CurrentUICulture = culture;

        services.AddDbContext<AppDbContext>(options =>
            {
                options
                    .UseSqlServer(GetDbConnection(configuration!, env),
                        op => op.MigrationsAssembly("PCA.Infrastructure"))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                    .EnableSensitiveDataLogging()
                    .UseLazyLoadingProxies();
            });

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddTransient<IWebSocketClient>(_ => new WebSocketClient(services));     
        services.AddTransient<ApiHttpClient>(_ => new ApiHttpClient(services));
        services.AddSingleton<IApiProducer, ApiEventProducer>(_ => new ApiEventProducer(services));
        services.AddHostedService<ApiEventConsumer>(_ => new ApiEventConsumer(services));
        services.AddSingleton<IMessageProcessor, MessageProcessor>(_ => new MessageProcessor(services));
        services.AddSingleton<ServicesMappingProfile>();
        services.AddHttpContextAccessor();
        
        try
        {
            IsInitializedDatabase(serviceProvider, logger);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while initializing or seeding the database.");
        }
    }

    private static string GetDbConnection(IConfiguration configuration, IHostEnvironment env)
    {
        return (env.IsDevelopment()
            ? configuration.GetConnectionString("LocalConnection")
            : configuration.GetConnectionString("ServerConnection"))!;
    }

    private static void IsInitializedDatabase(IServiceProvider sp, ILogger? logger)
    {
        using var scope = sp.GetService<IServiceScopeFactory>()!.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (dbContext.Database.CanConnect())
        {
            logger?.LogDebug($"There is a connection to the database: {dbContext.Database.ProviderName}/r/n Migrations started");
            dbContext.Database.Migrate();
            logger?.LogDebug("Migrations completed");
        }

        logger?.LogDebug($"There is no connection to the database: {dbContext.Database.ProviderName}");
    }
}