using WebApp.Data.enums;

namespace WebApp.Api.Dto;

public class ProjectsDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ArchivedStatus ArchivedStatus { get; set; }
}