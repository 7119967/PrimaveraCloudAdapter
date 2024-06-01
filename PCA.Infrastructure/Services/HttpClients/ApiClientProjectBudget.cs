namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientProjectBudget(IServiceCollection services) : BaseHttpClient<ApiClientProjectBudget>(services)
{
    public override async Task GetDataAsync(Transaction transaction, dynamic json)
    {
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
        await SaveData(transaction, data!);
    }

    protected override async Task InsertEntity(Transaction transaction, string eventDetails)
    {
        var entity = new ProjectBudget
        {
            TransactionId = transaction!.Id,
            Json = eventDetails
        };
        
        var entry = await _unitOfWork.ProjectBudgetRepository.Insert(entity, new CancellationToken());
    }
}