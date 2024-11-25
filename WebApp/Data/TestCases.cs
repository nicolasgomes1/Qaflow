using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestCases
{
    public int Id { get; set; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(500)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    public TestTypes TestType { get; set; }

    public TestScope TestScope { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TestPlans> TestPlans { get; set; } = new List<TestPlans>();

    public List<TestSteps> TestSteps { get; set; } = new();

    public TimeSpan EstimatedTime { get; set; } // Field to store the estimated time of the test case execution

    public ICollection<Requirements>? Requirements { get; set; } = new List<Requirements>();

    public ICollection<TestCaseExecution> TestCaseExecutions { get; set; } = new List<TestCaseExecution>();


    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }

    public int TcProjectId { get; set; }

    public Projects? Projects { get; set; }
    
    public WorkflowStatus WorkflowStatus { get; set; }
    
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;

    public ICollection<TestCasesJira> TestCasesJira { get; set; } = new List<TestCasesJira>();

    public ICollection<TestCasesFile> TestCasesFiles { get; set; } = new List<TestCasesFile>();
}