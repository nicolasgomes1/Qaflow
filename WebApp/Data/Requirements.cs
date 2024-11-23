using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Requirements
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(150)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public ArchivedStatus ArchivedStatus { get; init; } = ArchivedStatus.Active;

    public ICollection<TestCases>? TestCases { get; init; } = new List<TestCases>();

    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }

    public int RProjectId { get; set; }

    public Projects? Projects { get; init; }

    public ICollection<RequirementsFile> RequirementsFiles { get; set; } = new List<RequirementsFile>();
}