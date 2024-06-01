namespace PCA.Infrastructure.Services;

public class HealthCheckRemote: IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public HealthCheckRemote(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        using var httpClient = _httpClientFactory.CreateClient();
        var response = await httpClient.GetAsync("https://primavera-ca1.oraclecloud.com");
        if (response.IsSuccessStatusCode)
        {
            return HealthCheckResult.Healthy($"Remote endpoints is healthy.");
        }

        return HealthCheckResult.Unhealthy("Remote endpoint is unhealthy");
    }
}