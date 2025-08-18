using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestCases : BaseEntity
{
    public int Id { get; set; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(500)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public TestTypes TestType { get; set; }

    public TestScope TestScope { get; set; }

    public List<TestSteps> TestSteps { get; set; } = new();

    public TimeSpan EstimatedTime { get; set; } // Field to store the estimated time of the test case execution

    public int ProjectsId { get; set; }

    public Projects? Projects { get; set; }

    public WorkflowStatus WorkflowStatus { get; set; }

    [MaxLength(50)] public string AssignedTo { get; set; } = string.Empty;

    public ICollection<TestPlans> TestPlans { get; set; } = [];

    public ICollection<Requirements>? LinkedRequirements { get; set; } = [];

    public ICollection<TestCaseExecution> LinkedTestCaseExecutions { get; set; } = [];

    public ICollection<TestCasesJira> TestCasesJira { get; set; } = [];

    public ICollection<TestCasesFile> LinkedTestCasesFiles { get; set; } = [];

    public Bugs? Bugs { get; set; }
}