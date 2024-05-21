using HealthChecks.UI.Configuration;

namespace PCA.API;

/// <summary>
/// 
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        //Log.Information("Starting application");
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        
        builder.Services.ConfigureBusinessServices();
        builder.Services.ConfigureHealthChecks();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();
        //app.UseAuthorization();
        app.UseWebSockets();

        app.MapControllers();
        // app.UseMiddleware<ExceptionHandlerMiddleware>();
        
        app.MapHealthChecks("/api/health", new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
        app.UseHealthChecksUI(delegate (Options options) 
        {
            options.UIPath = "/healthcheck-ui";
            // options.AddCustomStylesheet("./HealthCheckRemote/Custom.css");

        });
        
        app.Run();
    }
}
