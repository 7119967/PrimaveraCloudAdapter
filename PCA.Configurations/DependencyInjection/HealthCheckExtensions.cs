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
                
            // var profile = launchSettings.Profiles.Values.FirstOrDefault(p => 
            //      p.EnvironmentVariables["LAUNCH_PROFILE"] == profileName);

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
    
    private static string? GetDbConnection(IConfiguration configuration, IHostEnvironment env)
    {
        return env.IsDevelopment()
            ? configuration.GetConnectionString("LocalConnection")
            : configuration.GetConnectionString("ServerConnection");
    }
    
    public static void ConfigureHealthChecks(this IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider(true);
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var env = serviceProvider.GetRequiredService<IHostEnvironment>();
        var connection = GetDbConnection(configuration!, env);
        var baseUrl = GetApplicationUrl(env);
        
        // services.Configure<HealthChecksUIOptions>(configuration.GetSection("HealthChecksUI"));
        services.AddHealthChecks()
            .AddSqlServer(connection!, 
                healthQuery: "select 1", 
                name: "SQL Server", 
                failureStatus: HealthStatus.Unhealthy, 
                tags: new[] { "Feedback", "Database" })
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
                opt.SetEvaluationTimeInSeconds(10); //time in seconds between check    
                opt.MaximumHistoryEntriesPerEndpoint(60); //maximum history of checks    
                opt.SetApiMaxActiveRequests(1); //api requests concurrency    
                // opt.AddHealthCheckEndpoint("feedback api", "/api/health"); //map health check api    

            })
            .AddInMemoryStorage();
    }
}