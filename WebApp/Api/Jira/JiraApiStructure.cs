namespace WebApp.Api.Jira;

public class JiraTask
{
    public string Id { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public Fields Fields { get; set; } = new();
}

public class Fields
{
    public string Summary { get; set; } = string.Empty;
    public IssueType Issuetype { get; set; } = new(); // Add IssueType property
}

public class IssueType
{
    public string Name { get; set; } = string.Empty; // "Bug", "Task", etc.
}