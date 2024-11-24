using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace WebApp.Api.Jira;

public class JiraService
{
    private readonly HttpClient _httpClient;

    public JiraService(HttpClient httpClient, IOptions<JiraApiOptions> connection)
    {
        var options = connection.Value;

        // Check if configuration values are populated
        if (string.IsNullOrWhiteSpace(options.BaseUrl) || string.IsNullOrWhiteSpace(options.Username) ||
            string.IsNullOrWhiteSpace(options.ApiKey))
        {
            throw new ArgumentException("Jira API configuration values are missing or invalid.");
        }

        // Set the base address for HttpClient
        httpClient.BaseAddress = new Uri(options.BaseUrl);

        // Set up Basic Authentication Header
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{options.Username}:{options.ApiKey}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        _httpClient = httpClient;
    }

    public async Task<List<JiraTask>> GetProjectIssuesAsync(string projectId)
    {
        // Define the endpoint path for Jira project issues
        var requestPath = $"/rest/api/3/search?jql=project={projectId}";

        var response = await _httpClient.GetAsync(requestPath);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Failed to fetch Jira issues: {response.StatusCode}, {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        // Deserialize the "issues" property in the JSON response to a list of JiraTask objects
        var tasks = jsonDocument.RootElement.GetProperty("issues").Deserialize<List<JiraTask>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tasks == null) throw new Exception("Error while deserializing json");

        return tasks;
    }
}