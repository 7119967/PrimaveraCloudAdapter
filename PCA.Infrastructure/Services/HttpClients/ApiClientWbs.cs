namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientWbs(IServiceCollection services) : BaseHttpClient<ApiClientWbs>(services)
{
    public override async Task GetDataAsync(Transaction transaction, dynamic json)
    {
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityWbsView>(json);
        var requestUri = $"/api/restapi/wbs/{apiEntity!.PrimaryKey}";
        var response = await HttpClient.SendRequestAsync(requestUri);

        if (response == null)
        {
            Logger.LogDebug($"{GetType().Name} reports: The response has no required data");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonConvert.DeserializeObject(jsonString);
        await SaveData(transaction, data!);
    }

    protected override async Task InsertEntity(Transaction transaction, string eventDetails)
    {
        var entity = new Wbs
        {
            TransactionId = transaction!.Id,
            Json = eventDetails
        };
        
        var entry = await UnitOfWork.WbsRepository.Insert(entity, new CancellationToken());
    }
}