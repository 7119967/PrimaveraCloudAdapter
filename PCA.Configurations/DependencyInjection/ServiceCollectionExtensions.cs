namespace PCA.Configurations.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void ConfigureBusinessServices(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("PCA Logger");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        Console.OutputEncoding = Encoding.UTF8;

        var connectionString = DbConnectionHelper.GetDbConnection(configuration, env);
        logger.LogInformation("Using connection string: {ConnectionString}", connectionString);

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString,
                op => op.MigrationsAssembly("PCA.Infrastructure"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
                .EnableSensitiveDataLogging()
                .UseLazyLoadingProxies();
        });

        services.AddHttpContextAccessor();
        services.AddSingleton<ServicesMappingProfile>();
        services.AddTransient<IUnitOfWork, UnitOfWork>();
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<ApiHttpClient>(_ => new ApiHttpClient(services));
        services.AddTransient<IWebSocketClient>(_ => new WebSocketClient(services));
        services.AddHostedService<ApiEventConsumer>(_ => new ApiEventConsumer(services));
        services.AddTransient<IApiProducer, ApiEventProducer>(_ => new ApiEventProducer(services));
        services.AddTransient<IMessageProcessor, MessageProcessor>(_ => new MessageProcessor(services));

        try
        {
            DbInitializer.IsInitializedDatabase(services, logger);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "An error occurred while initializing or seeding the database.");
        }
    }
}
