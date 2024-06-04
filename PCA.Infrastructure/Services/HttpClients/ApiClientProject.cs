namespace PCA.Infrastructure.Services.HttpClients;

public class ApiClientProject(IServiceCollection services) : BaseHttpClient<ApiClientProject>(services)
{
    public override async Task GetDataAsync(Transaction transaction, dynamic json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };
        var apiEntity = JsonSerializer.Deserialize<ApiEntityProjectView>(json, options);
        var requestUri = $"/api/restapi/project/{apiEntity!.PrimaryKey}";
        var response = await HttpClient.SendRequestAsync(requestUri);

        if (response == null)
        {
            Logger.LogDebug($"{GetType().Name} reports: The response has no required data");
            return;
        }

        var jsonString = await response.Content.ReadAsStringAsync();
        var data = JsonSerializer.Deserialize<dynamic>(jsonString, options);
        await SaveData(transaction, data!);
    }

    protected override async Task InsertEntity(Transaction transaction, string eventDetails)
    {
        var entity = new Project
        {
            TransactionId = transaction!.Id,
            Json = eventDetails
        };

        var entry = await UnitOfWork.ProjectRepository.Insert(entity, new CancellationToken());
    }
}