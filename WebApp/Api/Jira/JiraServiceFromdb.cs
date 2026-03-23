using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Api.Jira;

public class JiraServiceFromDb(HttpClient httpClient, IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    /// <summary>
    /// Retrieves Jira issues for a given project from Jira using the service configured from DB.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve from DB.</param>
    /// <param name="jiraProjectId">The Jira project ID for which to fetch issues.</param>
    /// <returns>Returns a list of JiraTask objects representing the issues.</returns>
    public async Task<List<JiraTask>> GetProjectIssuesFromDbAsync(string uniqueKey, string jiraProjectId)
    {
        if (string.IsNullOrWhiteSpace(uniqueKey) || string.IsNullOrWhiteSpace(jiraProjectId))
        {
            return [];
        }

        try
        {
            // Retrieve integration configuration from DB
            var integration = await GetIntegrationByUniqueKeyAsync(uniqueKey);

            if (integration == null)
            {
                return [];
            }

            // Ensure that required values (BaseUrl, Username, ApiKey) are available
            if (string.IsNullOrWhiteSpace(integration.BaseUrl) ||
                string.IsNullOrWhiteSpace(integration.Username) ||
                string.IsNullOrWhiteSpace(integration.ApiKey))
            {
                return [];
            }

            // Configure the HttpClient with the database values
            httpClient.BaseAddress = new Uri(integration.BaseUrl);
            var authToken =
                System.Convert.ToBase64String(Encoding.UTF8.GetBytes($"{integration.Username}:{integration.ApiKey}"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", authToken);

            // Now call the API to get the Jira issues
            return await GetProjectIssuesAsync(jiraProjectId);
        }
        catch (Exception)
        {
            // Don't crash the app if integration fails
            return [];
        }
    }

    /// <summary>
    /// Retrieves Jira issues for a project by its internal project ID.
    /// </summary>
    /// <param name="internalProjectId">The internal ID of the project.</param>
    /// <returns>Returns a list of JiraTask objects.</returns>
    public async Task<List<JiraTask>> GetJiraIssuesByInternalProjectIdAsync(int internalProjectId)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        var project = await dbContext.Projects
            .Include(p => p.JiraIntegration)
            .FirstOrDefaultAsync(p => p.Id == internalProjectId);

        if (project == null || string.IsNullOrWhiteSpace(project.JiraProjectId) || project.JiraIntegration == null)
        {
            return [];
        }

        return await GetProjectIssuesFromDbAsync(project.JiraIntegration.UniqueKey, project.JiraProjectId);
    }

    /// <summary>
    /// Retrieves an integration by its unique key from the database.
    /// </summary>
    /// <param name="uniqueKey">The unique key of the integration to retrieve.</param>
    /// <returns>Returns the integration with the specified unique key, or null if not found.</returns>
    private async Task<Integrations?> GetIntegrationByUniqueKeyAsync(string uniqueKey)
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        return await dbContext.Integrations
            .FirstOrDefaultAsync(x => x.UniqueKey == uniqueKey);
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
            throw new HttpRequestException($"Failed to fetch Jira issues: {response.StatusCode}, {errorContent}");
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