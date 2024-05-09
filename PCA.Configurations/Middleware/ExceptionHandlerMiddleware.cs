namespace PCA.Configurations.Middleware;

public class ExceptionHandlerMiddleware
{
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    private readonly RequestDelegate _next;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="logger"></param>
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BaseException ex)
        {
            _logger.LogError(ex.Message);
            await HandleBaseExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync($"Internal Server Error: {exception.Message}");
    }

    private async Task HandleBaseExceptionAsync(HttpContext context, BaseException exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = exception.Code;
        await context.Response.WriteAsync(exception.Message);
    }
}