namespace PCA.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/subscriptions")]
public class SubscriptionsController : Controller
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApiProducer _apiProducer;
    private readonly ILogger<SubscriptionsController> _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="mapper"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="apiProducer"></param>
    public SubscriptionsController(
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IApiProducer apiProducer,
        ILogger<SubscriptionsController> logger
    )
    {
        _logger = logger;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _apiProducer = apiProducer;
    }

    [HttpPost("")]
    public async Task<IActionResult> Add([FromBody] SubscriptionView model, CancellationToken ctn = default)
    {
        var context = new ValidationContext(model);
        var results = new List<ValidationResult>();
        if (!Validator.TryValidateObject(model, context, results, true))
        {
            Console.WriteLine("Failed to validate the SubscriptionView object");
            foreach (var error in results)
            {
                _logger.LogInformation(error.ErrorMessage);
            }
            _logger.LogInformation("");
        }
        else
            _logger.LogInformation($"The SubscriptionView object was validated successfully. Name: {model.EntityObjectType}\n");
       
        try
        {
            var message = GetJsonObject(model).ToString();
            await UpdateOrInsert(model, ctn);
            var response = await _apiProducer.SendAsync(message);
            return Ok(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message, e);
            throw new BaseException("An error occurred while adding the Subscription");
        }
    }

    [HttpGet("id/{id}")]
    public async Task<IActionResult> GetById(long id, CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.SubscriptionRepository.GetById(id, ctn);

        if (entity is null)
        {
            _logger.LogInformation("Entity does not exist");
            throw new BaseException("Entity does not exist");
        }

        return Ok(entity);
    }

    [HttpGet("")]
    public async Task<IActionResult> GetAll(CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.SubscriptionRepository.GetAll(ctn);

        if (entity is not null) return Ok(entity);
        _logger.LogInformation("Entity does not exist");
        throw new BaseException("Entity does not exist");
    }

    [HttpPut("")]
    public async Task<IActionResult> Edit([FromBody] Subscription entity, CancellationToken ctn = default)
    {
        var model = _mapper.Map<SubscriptionView>(entity);
        var message = JsonConvert.SerializeObject(GetJsonObject(model));
        await _apiProducer.SendAsync(message);
        var entry = await _unitOfWork.SubscriptionRepository.GetById(entity.Id, ctn);

        if (entry != null)
        {
            await _unitOfWork.SubscriptionRepository.Update(entity, ctn);
            return Ok("Entity updated successfully");
        }

        _logger.LogInformation("Entity does not exist");
        throw new BaseException("Entity does not exist");
    }

    [HttpDelete("id/{id}")]
    public async Task<IActionResult> Delete(long id, CancellationToken ctn = default)
    {
        var entity = await _unitOfWork.SubscriptionRepository.GetById(id, ctn);

        if (entity != null)
        {
            var model = _mapper.Map<SubscriptionView>(entity);
            model.IsEnabled = false;
            var message = JsonConvert.SerializeObject(GetJsonObject(model));
            await _apiProducer.SendAsync(message);
            await _unitOfWork.SubscriptionRepository.Delete(id, ctn);
            return Ok("Entity removed successfully");
        }

        _logger.LogInformation("Entity does not exist");
        throw new BaseException("Entity does not exist");
    }

    private JObject GetJsonObject(SubscriptionView model)
    {
        return new JObject(
            new JProperty("subscription", model.IsEnabled),
            new JProperty("entityObjectType", model.EntityObjectType),
            new JProperty("eventTypes", JArray.FromObject(model.EventTypes)),
            new JProperty("filters", model.Filters)
        );
    }

    private async Task UpdateOrInsert(SubscriptionView model, CancellationToken ctn)
    {
        var entity = _mapper.Map<Subscription>(model);
        var result = _unitOfWork.SubscriptionRepository.GetNoTracking()
            .FirstOrDefault(e => e.EntityObjectType == entity.EntityObjectType);
        if (result != null)
        {
            await _unitOfWork.SubscriptionRepository.Update(entity, ctn);
        }
        else
        {
            await _unitOfWork.SubscriptionRepository.Insert(entity, ctn);
        }
    }
}