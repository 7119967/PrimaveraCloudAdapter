namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientRelationship(IServiceCollection services) : BaseHttpClient<ApiClientRelationship>(services)
{
    public override async Task GetDataAsync(Transaction transaction, dynamic json)
    {
        var apiEntity = JsonConvert.DeserializeObject<ApiEntityRelationshipView>(json);
        var requestUri = $"/api/restapi/relationship/{apiEntity!.PrimaryKey}";
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
        var entity = new ActivityRelationship
        {
            TransactionId = transaction!.Id,
            Json = eventDetails
        };
        
        var entry = await UnitOfWork.ActivityRelationshipRepository.Insert(entity, new CancellationToken());
    }
}