namespace PCA.API.Controllers;

[ApiController]
[Route("api/healthcheck")]
public class HealthCheckController : ControllerBase
{
    private readonly HealthCheckService _healthCheckService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="healthCheckService"></param>
    /// <param name="httpContextAccessor"></param>
    public HealthCheckController(HealthCheckService healthCheckService, IHttpContextAccessor httpContextAccessor)
    {
        _healthCheckService = healthCheckService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpGet("healthcheck/ready")]
    public async Task<ActionResult<string>> HealthCheckReady()
    {
        var result = await HealthCheckResult();
        return Content(result, "application/json");
    }

    [HttpGet("healthcheck/live")]
    public async Task<ActionResult<string>> HealthCheckLive()
    {
        var result = await Task.Run(HealthCheckResultUi);
        return Content(result, "application/json");
    }

    private async Task<string> HealthCheckResult()
    {
        var healthCheckResult = await _healthCheckService.CheckHealthAsync();
        var result = JsonConvert.SerializeObject(new
        {
            status = healthCheckResult.Status.ToString(),
            errors = healthCheckResult.Entries.Select(e => 
                new
                {
                    key = e.Key, 
                    value = e.Value.Status.ToString()
                })
        });

        return result;
    }
    
    private async Task<string?> HealthCheckResultUi()
    {
        var health = new HealthCheckOptions()
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        };

        var httpContext = _httpContextAccessor.HttpContext;
        var healthReport = await _healthCheckService.CheckHealthAsync();
        if (httpContext != null)
        {
            var result = UIResponseWriter.WriteHealthCheckUIResponse(httpContext, healthReport).ToString();
            return result;
        }

        var result2 = BadRequest();
        return (string)(object)result2;
    }
}