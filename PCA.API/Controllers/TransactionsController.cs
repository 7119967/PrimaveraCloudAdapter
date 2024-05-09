namespace PCA.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/transactions")]
public class TransactionsController : Controller
{
    private readonly ILogger _logger;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWebSocketClient _webSocket;
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="webSocket"></param>
    public TransactionsController(
        ILogger<TransactionsController> logger, 
        IMapper mapper, 
        IUnitOfWork unitOfWork, 
        IWebSocketClient webSocket)
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _webSocket = webSocket;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.TransactionRepository.GetAll(ctn); 
        
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
        var entity = await _unitOfWork.TransactionRepository.GetById(id, ctn); 
        
        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            return NotFound("Entity does not exist");
        }

        return Ok(entity);
    }
}