using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Requirements : BaseEntity
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(150)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public int ProjectsId { get; set; }

    public Projects? Projects { get; init; }


    public WorkflowStatus WorkflowStatus { get; set; }

    [MaxLength(50)] public string AssignedTo { get; set; } = string.Empty;

    public ICollection<TestCases>? LinkedTestCases { get; init; } = [];
    public ICollection<RequirementsFile> LinkedRequirementsFiles { get; set; } = [];

    public RequirementsSpecification? RequirementsSpecification { get; set; }
    public bool HasTestCases => LinkedTestCases != null && LinkedTestCases.Any();
    public string HasTestCasesText => HasTestCases ? "Yes" : "No";
}