using PCA.Configurations.Application;
using PCA.Infrastructure.Services.HealthChecks;

namespace PCA.Configurations.DependencyInjection;
public class LaunchSettings
{
    public Dictionary<string, Profile>? Profiles { get; set; }
}

public class Profile
{
    public string? CommandName { get; set; }
    public bool LaunchBrowser { get; set; }
    public string? LaunchUrl { get; set; }
    public Dictionary<string, string>? EnvironmentVariables { get; set; }
    public bool DotnetRunMessages { get; set; }
    public string? ApplicationUrl { get; set; }
}

public static class HealthCheckExtensions
{
    private static string? GetApplicationUrl(IHostEnvironment env)
    {
        var applicationUrl = string.Empty;
        
        var profileName = Environment.GetEnvironmentVariable("LAUNCH_PROFILE");
        if (string.IsNullOrEmpty(profileName))
        {
            Console.WriteLine("LAUNCH_PROFILE environment variable not set.");
            return applicationUrl;
        }

        var launchSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json");

        if (File.Exists(launchSettingsPath))
        {
            var launchSettingsJson = File.ReadAllText(launchSettingsPath);
            var launchSettings = JsonSerializer.Deserialize<LaunchSettings>(launchSettingsJson, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            foreach (var profile in launchSettings!.Profiles!.Values)
            {
                if (profile.EnvironmentVariables!["LAUNCH_PROFILE"] == profileName)
                {
                    applicationUrl = profile.ApplicationUrl;
                    Console.WriteLine($"ApplicationUrl for profile {profileName}: {applicationUrl}");
                    break;
                }
                else
                {
                    Console.WriteLine($"Profile {profileName} not found in launchSettings.json");
                }
            }
        }
        else
        {
            Console.WriteLine("launchSettings.json file not found");
        }
        
        return applicationUrl;
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var connection = DbConnectionHelper.GetDbConnection(configuration, env);
        //var baseUrl = GetApplicationUrl(env);

        // services.Configure<HealthChecksUIOptions>(configuration.GetSection("HealthChecksUI"));
        services.AddSingleton<HealthCheckDatabase>(new HealthCheckDatabase(connection!));
        services.AddHealthChecks()
            .AddSqlServer(
                connectionString: connection!,
                healthQuery: "SELECT 1;",
                name: "Database", 
                failureStatus: HealthStatus.Unhealthy,
                timeout: TimeSpan.FromSeconds(5),
                tags: new[] { "Feedback", "Database" })
            .AddCheck<HealthCheckDatabase>("Database Extended")
            .AddCheck<HealthCheckRemote>(
                "Remote endpoints Health Check", 
                failureStatus: HealthStatus.Unhealthy, 
                tags: new[] { "Primavera", "OracleCloud" })
            .AddCheck<HealthCheckMemory>(
                $"Feedback Service Memory Check", 
                failureStatus: HealthStatus.Unhealthy, 
                tags: new[] { "Memory" });
            // .AddUrlGroup(
            //     new Uri($"{baseUrl}/api/v1/heartbeats/ping"), 
            //     httpMethod: HttpMethod.Post,
            //     name: "base URL", 
            //     failureStatus: HealthStatus.Unhealthy,
            //     tags: new[] { "http" });

        //services.AddHealthChecksUI();
        services.AddHealthChecksUI(opt =>
            {
                opt.SetEvaluationTimeInSeconds(60); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                // opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
            .AddInMemoryStorage();
    }
}