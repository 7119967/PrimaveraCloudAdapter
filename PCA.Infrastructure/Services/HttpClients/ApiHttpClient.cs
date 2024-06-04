namespace PCA.Infrastructure.Services.HttpClients;

public class ApiHttpClient
{
    private string? _url;
    private string? PASSWORD;
    private string? HOST_NAME;
    private string? USER_NAME;
    private readonly IServiceScope? _scope;
    private ILogger<ApiHttpClient> _logger;
    private readonly HttpClient _httpClient;
    private readonly IHttpClientStrategy<HttpResponseMessage> _httpClientStrategy;

    public ApiHttpClient(IServiceCollection services)
    {
        var serviceProvider = services.BuildServiceProvider();
        var scope = serviceProvider.CreateScope();
        Initiate(scope);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 1000
        };
        _httpClient = new HttpClient(socketsHttpHandler);
    }
    
    public ApiHttpClient(IServiceScope scope, IHttpClientStrategy<HttpResponseMessage> httpClientStrategy)
    {
        _scope = scope;
        _httpClientStrategy = httpClientStrategy;
        Initiate(_scope);
        var socketsHttpHandler = new SocketsHttpHandler
        {
            MaxConnectionsPerServer = 1000
        };
        _httpClient = new HttpClient(socketsHttpHandler);
    }

    private void Initiate(IServiceScope scope)
    {
        _logger = scope.ServiceProvider.GetRequiredService<ILogger<ApiHttpClient>>();
        var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        _url = GetBaseUri(configuration);
        HOST_NAME = configuration["PrimaveraCloudApi:HostName"]!;
        USER_NAME = configuration["PrimaveraCloudApi:UserName"]!;
        PASSWORD = configuration["PrimaveraCloudApi:Password"]!;
    }

    private string GetBaseUri(IConfiguration configuration)
    {
        var versionApi = configuration["PrimaveraCloudApi:VersionApi"];
        var baseUri = configuration["PrimaveraCloudApi:Url"];
        _url = string.IsNullOrEmpty(versionApi) ? $"{baseUri}" : $"{baseUri}/{versionApi}";
        return _url;
    }

    public async Task ExecuteRequests(Transaction transaction, dynamic message)
    {
        try
        {
            await _httpClientStrategy.GetDataAsync(transaction, message);
        }
        catch (Exception ex) when (ex is NullReferenceException or HttpRequestException)
        {
            _logger.LogError($"{GetType().Name} via {_url} received a message : {ex.Message}");
        }
    }

    public async Task<HttpResponseMessage?> SendRequestAsync(dynamic queryParams)
    {
        HttpResponseMessage response;
        var fullUrl = new Uri(_url + queryParams);
        var request = new HttpRequestMessage(HttpMethod.Get, fullUrl);
        var authTokenResponse = await GetAuthTokenDetails();
        SetCredentials(authTokenResponse!);
        request.Content = new StringContent("", Encoding.UTF8, "application/json");

        try
        {
            response = await _httpClient.SendAsync(request);
            _logger.LogDebug($"{GetType().Name} sent a request on {fullUrl}");

            if (!response!.IsSuccessStatusCode)
            {
                _logger.LogDebug(
                    $"returned the response status: {(int)response.StatusCode}, " +
                    $"reason: {response.ReasonPhrase}");

                if ((int)response.StatusCode == 401)
                {
                    _logger.LogInformation($"Check your token for {GetType().Name}");
                    return null;
                }

                return null;
            }

            return response;
        }
        catch (Exception ex) when (ex is NullReferenceException or HttpRequestException)
        {
            _logger.LogDebug($"returned the null response");
            _logger.LogDebug(ex.Message, ex);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError($"{GetType().Name} via {fullUrl} received a message: {e.Message}");
            throw;
        }
    }
    
    public async Task<Dictionary<string, object>?> GetAuthTokenDetails()
    {
        var requestUri = $"https://{HOST_NAME}/primediscovery/apitoken/request?scope=http://{HOST_NAME}/api";
        var response = await SendRequestAsync(requestUri);
        if (response == null) return null;

        var jsonString = await response.Content.ReadAsStringAsync();
        var dataList = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonString);
        return dataList;
    }
    
    private async Task<HttpResponseMessage?> SendRequestAsync(string requestUri)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
        var credentials = $"{USER_NAME}:{PASSWORD}";
        var credentialsBase64 = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialsBase64);
        try
        {
            var response = await _httpClient.SendAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                if ((int)response.StatusCode == 401) return null;
                return null;
            }

            return response;
        }
        catch (Exception ex) when (ex is NullReferenceException or HttpRequestException)
        {
            _logger.LogDebug(ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex.Message);
            throw;
        }
    }

    private static string Base64Encode(string username, string password)
    {
        var credentials = $"{username}:{password}";
        var credentialsBytes = Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(credentialsBytes);
    }
    
    private void SetCredentials(Dictionary<string, object> authTokenResponse)
    {
        var tokenAuth = $"Bearer {authTokenResponse["accessToken"]}";
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authTokenResponse["accessToken"].ToString());
        var json = JsonSerializer.Serialize(authTokenResponse["requestHeaders"]);
        var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        foreach (var header in headers!)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value!.ToString());
        }
    }
}