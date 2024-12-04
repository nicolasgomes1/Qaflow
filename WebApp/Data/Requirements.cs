using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Requirements : BaseEntity
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(150)] public string? Description { get; set; }

    public Priority Priority { get; set; }
    
    public ICollection<TestCases>? TestCases { get; init; } = new List<TestCases>();
    
    public int ProjectsId { get; set; }

    public Projects? Projects { get; init; }

    public ICollection<RequirementsFile> RequirementsFiles { get; set; } = new List<RequirementsFile>();
    
    public WorkflowStatus WorkflowStatus { get; set; }
    
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;

}