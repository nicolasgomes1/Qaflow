namespace WebApp.Api.Jira;

public class JiraApiOptions
{
    /// <summary>
    /// Base class to provide the Base Url from which the api will be called
    /// Sample Base Url https://your-domain.atlassian.net
    /// </summary>
    public required string BaseUrl { get; init; }

    /// <summary>
    /// Sample Username test@example.com
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Sample ApiKey api_secret_key created from Jira
    /// </summary>
    public required string ApiKey { get; init; }
}