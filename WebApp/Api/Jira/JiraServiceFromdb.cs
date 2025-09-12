using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Api.Jira;

public class JiraServiceFromDb(HttpClient httpClient, IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(JiraServiceFromDb));

    
    /// <summary>
    /// Retrieves Jira issues for a given project from Jira using the service configured from DB.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve from DB.</param>
    /// <param name="jiraprojectId">The Jira project ID for which to fetch issues.</param>
    /// <returns>Returns a list of JiraTask objects representing the issues.</returns>
    public async Task<List<JiraTask>> GetProjectIssuesFromDbAsync(string uniqueKey, string jiraprojectId)
    {
        // Retrieve integration configuration from DB
        var integration = await GetIntegrationByUniqueKeyAsync(uniqueKey);

        // Ensure that required values (BaseUrl, Username, ApiKey) are available
        if (string.IsNullOrWhiteSpace(integration.BaseUrl) ||
            string.IsNullOrWhiteSpace(integration.Username) ||
            string.IsNullOrWhiteSpace(integration.ApiKey))
        {
            Logger.LogError("Jira API configuration values are missing or invalid.");
        }

        // Configure the HttpClient with the database values
        httpClient.BaseAddress = new Uri(integration.BaseUrl);
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{integration.Username}:{integration.ApiKey}"));
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

        // Now call the API to get the Jira issues
        return await GetProjectIssuesAsync(jiraprojectId);
    }

    /// <summary>
    /// Retrieves an integration by its unique key from the database.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified unique key.</returns>
    /// <exception cref="Exception">Thrown when the integration is not found.</exception>
    private async Task<Integrations> GetIntegrationByUniqueKeyAsync(string uniqueKey)
    {
        
        
        using var dbContext = dbContextFactory.CreateDbContext();
        var integration = await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.UniqueKey == uniqueKey);

        if (integration == null)
        {
            Logger.LogTrace("Integration with key '{UniqueKey}' not found.", uniqueKey);
        }

        return integration;
    }

    /// <summary>
    /// Fetches the Jira project issues from Jira API.
    /// </summary>
    /// <param name="projectId">The Jira project ID to get issues for.</param>
    /// <returns>Returns a list of JiraTask objects.</returns>
    private async Task<List<JiraTask>> GetProjectIssuesAsync(string projectId)
    {
        var requestPath = $"/rest/api/3/search?jql=project={projectId}";

        var response = await httpClient.GetAsync(requestPath);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Logger.LogError("Failed to fetch Jira issues: {StatusCode}, {ErrorContent}", response.StatusCode, errorContent);
            return [];
           //TODO NEED TO SEE WTF THE ISSUE WITH THE API
            // throw new HttpRequestException($"Failed to fetch Jira issues: {response.StatusCode}, {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();
        var jsonDocument = JsonDocument.Parse(content);

        // Deserialize the "issues" property into a list of JiraTask objects
        var tasks = jsonDocument.RootElement.GetProperty("issues").Deserialize<List<JiraTask>>(new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (tasks == null) throw new Exception("Error while deserializing JSON.");

        return tasks;
    }
}