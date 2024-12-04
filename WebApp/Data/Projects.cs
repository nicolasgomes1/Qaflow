using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Projects : BaseEntity
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string Name { get; set; } = string.Empty;

    [MaxLength(100)] [Required] public string Description { get; set; } = string.Empty;

    
    public ICollection<Requirements> Requirements { get; init; } = new List<Requirements>();
    public ICollection<RequirementsFile> RequirementsFile { get; init; } = new List<RequirementsFile>();
    public ICollection<TestPlans> TestPlans { get; init; } = new List<TestPlans>();
    public ICollection<TestPlansFile> TestPlansFile { get; init; } = new List<TestPlansFile>();

    public ICollection<TestCases> TestCases { get; init; } = new List<TestCases>();

    public ICollection<TestCasesFile> TestCasesFile { get; init; } = new List<TestCasesFile>();
    public ICollection<TestExecution> TestExecution { get; init; } = new List<TestExecution>();

    public ICollection<TestStepsExecutionFile> TestStepsExecutionFile { get; init; } =
        new List<TestStepsExecutionFile>();

    public ICollection<Bugs> Bugs { get; init; } = new List<Bugs>();

    public ICollection<BugsFiles> BugsFile { get; init; } = new List<BugsFiles>();
}