namespace PCA.Infrastructure.Services;

public class SubscriptionResumer : BackgroundService
{
    private readonly IHelper _helper;
    private readonly IApiProducer _apiEventProducer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SubscriptionResumer> _logger;

    public SubscriptionResumer(IServiceCollection services)
    {
        _serviceProvider = services.BuildServiceProvider();
        _helper = _serviceProvider.GetRequiredService<IHelper>();
        _apiEventProducer = _serviceProvider.GetRequiredService<IApiProducer>();
        _logger = _serviceProvider.GetRequiredService<ILogger<SubscriptionResumer>>();
    }

    public override async Task StartAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SubscriptionResumer is starting.");
        await RestoreSubscriptionsAsync(stoppingToken);
        _logger.LogInformation("SubscriptionResumer has completed its background work and is stopping.");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("SubscriptionResumer is stopping.");
        return Task.CompletedTask;
    }

    private async Task RestoreSubscriptionsAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        List<Subscription> subscriptions = await uow.SubscriptionRepository
            .GetNoTracking()
            .Where(s => s.IsEnabled).ToListAsync(stoppingToken);

        foreach (var subscription in subscriptions)
        {
            var model = new SubscriptionView
            {
                IsEnabled = subscription.IsEnabled,
                EntityObjectType = subscription.EntityObjectType,
                EventTypes = subscription.EventTypes,
                Filters = subscription.Filters
            };

            var message = _helper.GetJsonObject(model).ToString();
            
            await Task.Delay(5000);
            var response = await _apiEventProducer.SendAsync(message);
            _logger.LogInformation(response);
        }
    }
}
