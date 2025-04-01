using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Projects : BaseEntity
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string Name { get; set; } = string.Empty;

    [MaxLength(100)] [Required] public string Description { get; set; } = string.Empty;


    public ICollection<Requirements> Requirements { get; init; } = [];
    public ICollection<RequirementsFile> RequirementsFile { get; init; } = [];
    public ICollection<RequirementsSpecification> RequirementsSpecification { get; init; } = [];

    public ICollection<TestPlans> TestPlans { get; init; } = [];
    public ICollection<TestPlansFile> TestPlansFile { get; init; } = [];

    public ICollection<TestCases> TestCases { get; init; } = [];
    public ICollection<TestCasesFile> TestCasesFile { get; init; } = [];

    public ICollection<TestExecution> TestExecution { get; init; } = [];
    public ICollection<TestStepsExecutionFile> TestStepsExecutionFile { get; init; } = [];

    public ICollection<Bugs> Bugs { get; init; } = [];
    public ICollection<BugsFiles> BugsFile { get; init; } = [];
}