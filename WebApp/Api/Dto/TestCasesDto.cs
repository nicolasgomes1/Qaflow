using WebApp.Data.enums;

namespace WebApp.Api.Dto;

public class TestCasesDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public Priority Priority { get; set; }
    public ArchivedStatus ArchivedStatus { get; set; }
    public ICollection<RequirementsDto> RequirementsDto { get; set; } = [];
    public int ProjectId { get; set; }
}