namespace PCA.Infrastructure.Services.HealthChecks;

public class HealthCheckDatabase : IHealthCheck
{
    private readonly string _connectionString;

    public HealthCheckDatabase(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken);
                return HealthCheckResult.Healthy("SQL Server is healthy");
            }
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server health check failed", ex);
        }
    }
}
