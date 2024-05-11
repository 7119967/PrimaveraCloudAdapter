namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientActivity(IServiceCollection services) : BaseHttpClient<ApiClientActivity>(services)
{
    public override async Task GetDataAsync(EventNotification eventNotification, dynamic json)
    {
        _eventNotification = eventNotification;
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityActivityView>(json);
        var requestUri = $"/api/restapi/activity/{apiEntity!.PrimaryKey}";

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
        var entity = new Activity
        {
            EventId = _eventNotification!.Id,
            Json = eventDetails
        };

        await _unitOfWork.ActivityRepository.Insert(entity, new CancellationToken());
    }
}