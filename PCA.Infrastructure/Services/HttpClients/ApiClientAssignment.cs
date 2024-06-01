namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientAssignment(IServiceCollection services) : BaseHttpClient<ApiClientAssignment>(services)
{
    public override async Task GetDataAsync(Transaction transaction, dynamic json)
    {
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityResourceAssignmentView>(json);
        var requestUri = $"/api/restapi/assignment/{apiEntity!.PrimaryKey}";
        var response = await _httpClient.SendRequestAsync(requestUri);

        if (response == null)
        {
            _logger.LogDebug($"{GetType().Name} reports: The response has no required data");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject(jsonString);
        await SaveData(transaction, data!);
    }

    protected override async Task InsertEntity(Transaction transaction, string eventDetails)
    {
        var entity = new ResourceAssignment
        {
            TransactionId = transaction!.Id,
            Json = eventDetails
        };
        
        var entry = await _unitOfWork.ResourceAssignmentRepository.Insert(entity, new CancellationToken());
    }
}