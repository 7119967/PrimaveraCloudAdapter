namespace PCA.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/eventnotifications")]
public class EventNotificationsController : Controller
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;

    public EventNotificationsController(ILogger<EventNotificationsController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.EventNotificationRepository.GetAll(ctn);

        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.EventNotificationRepository.GetById(id, ctn);

        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }

    [HttpGet("subscriptionid/{id}")]
    public async Task<IActionResult> GetBySubscriptionId(long id, CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.EventNotificationRepository.GetTracking()
            .FirstOrDefaultAsync(e => e.SubscriptionId == id, ctn);

        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }
}
