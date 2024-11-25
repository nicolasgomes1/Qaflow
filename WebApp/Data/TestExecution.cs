using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestExecution
{
    public int Id { get; set; }

    [Required] public string? Name { get; set; } = string.Empty; // Ensure Name is always set

    public string? Description { get; set; }

    public Priority Priority { get; set; }

    // Foreign key reference to TestPlan
    [Required] public int TestPlanId { get; set; }

    public TestPlans? TestPlan { get; set; } // Nullable reference to TestPlan

    // List of TestCase IDs related to the TestCaseExecution
    public List<int> SelectedTestCaseIds { get; set; } = [];

    // enum with values Passed, Failed, Blocked, NotRun
    public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.NotRun;

    // Version of the test case execution that can be Active or Archived
    // Archived Versions cannot be executed
    public bool IsActive { get; set; } = true;

    // The first execution start at version1 as we continue execution the version will be incremented and the status will be updated to Archived
    public int Version { get; set; }

    public TimeSpan Duration { get; set; } // Field to store the duration of the test execution
    public TimeSpan EstimatedTime { get; set; } // Field to store the estimated time of the test execution

    public string ExecutionNotes { get; set; } = string.Empty; // Field to store the notes of the test execution

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation property to TestCaseExecution
    public ICollection<TestCaseExecution> TestCaseExecutions { get; set; } = new List<TestCaseExecution>();

    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [MaxLength(50)]
    public string? ModifiedBy { get; set; }

    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;

    public int TEProjectId { get; set; }
    
    

    public Projects? Projects { get; set; }
}