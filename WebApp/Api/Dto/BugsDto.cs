namespace WebApp.Api.Dto;

public class BugsDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }

    public string Priority { get; set; } = string.Empty;

    public string Severity { get; set; } = String.Empty;

    public string BugStatus { get; set; } = string.Empty;
    public string ArchivedStatus { get; set; } = string.Empty;

    public int ProjectId { get; set; }
}