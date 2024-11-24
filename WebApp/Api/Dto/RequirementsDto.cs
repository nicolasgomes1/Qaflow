using WebApp.Data.enums;

namespace WebApp.Api.Dto;

public class RequirementsDto
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; } = string.Empty;
    public string Priority { get; set; } = string.Empty;
    public ArchivedStatus ArchivedStatus { get; set; }

    public int ProjectId { get; set; }
}