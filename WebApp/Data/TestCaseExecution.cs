using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestCaseExecution
{
    public int Id { get; set; }

    // Foreign key reference to TestExecution
    public int TestExecutionId { get; set; }
    public TestExecution? TestExecution { get; set; } // Navigation property to TestExecution

    public int TestCaseId { get; set; }
    public TestCases? TestCases { get; set; }// Navigation property to TestCase

    public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.NotRun;

    // Version of the test case execution, starting at 1 and incrementing
    public int Version { get; set; }

    // Indicates whether this version of the test case execution is active or archived
    public bool IsActive { get; set; } = true;

    public TimeSpan Duration { get; set; } // Duration of the test case execution

    public string ExecutionNotes { get; set; } = string.Empty; // Notes for the test case execution

    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    public ICollection<TestStepsExecution>? TestStepsExecution { get; set; } = new List<TestStepsExecution>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)]
    public string? CreatedBy { get; set; }

    [MaxLength(50)]
    public string? ModifiedBy { get; set; }

}