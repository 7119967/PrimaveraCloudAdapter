namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientProjectBudget(IServiceCollection services) : BaseHttpClient<ApiClientProjectBudget>(services)
{
    public override async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityProjectBudgetView>(json);
        var requestUri = $"/api/restapi/projectBudget/{apiEntity!.PrimaryKey}";

        var response = await _httpClient.SendRequestAsync(requestUri);

        if (response == null)
        {
            _logger.LogDebug($"{GetType().Name} reports: The response has no required data");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject(jsonString);
        await SaveData(data!);
    }

    protected override async Task InsertEntity(string eventDetails)
    {
        var entity = new ProjectBudget
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };
        
        await _unitOfWork.ProjectBudgetRepository.Insert(entity, new CancellationToken());
    }
}