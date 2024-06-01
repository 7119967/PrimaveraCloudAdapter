namespace PCA.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/resourceassignments")]
public class ResourceAssignmentsController : Controller
{
    private readonly ILogger _logger;
    private readonly IUnitOfWork _unitOfWork;
    
    public ResourceAssignmentsController(ILogger<ActivitiesController> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.ResourceAssignmentRepository.GetAll(ctn); 
        
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
        var entity = await _unitOfWork.ResourceAssignmentRepository.GetById(id, ctn); 
        
        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }
    
    [HttpGet("transactionid/{id}")]
    public async Task<IActionResult> GetByTransactionId(long id, CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.ResourceAssignmentRepository.GetTracking()
            .FirstOrDefaultAsync(e => e.TransactionId == id, ctn); 
        
        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }
}