using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Integrations
{
    public int Id { get; set; }
    [Required] public IntegrationType IntegrationType { get; set; }

    /// <summary>
    /// Base class to provide the Base Url from which the api will be called
    /// Sample Base Url https://your-domain.atlassian.net
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// Sample Username test@example.com
    /// </summary>
    [MaxLength(50)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Sample ApiKey api_secret_key created from Jira or other services
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Unique key to identify the integration if more than one available for the same type
    /// </summary>
    [MaxLength(50)]
    [Required]
    public string UniqueKey { get; set; } = string.Empty;
    
    /// <summary>
    /// Unique key to identify the Project Name in Jira if more than one available for the same type normally like WEB or API
    /// </summary>
    [MaxLength(50)]
    [Required]
    public string JiraProjectKey { get; set; } = string.Empty;
    
    public int ProjectsId { get; set; }
    public Projects? Projects { get; set; }
}