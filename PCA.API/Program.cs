namespace PCA.API;

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

        app.Run();
    }
}
