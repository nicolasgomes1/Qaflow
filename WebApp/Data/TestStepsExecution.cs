using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestStepsExecution
{
    public int Id { get; set; }

    // Foreign key reference to TestExecution
    public int TestCaseExecutionId { get; set; }
    public TestCaseExecution? TestCaseExecution { get; set; } // Navigation property to TestExecution

    public int TestStepsId { get; init; } // Foreign key reference to TestSteps
    
    [StringLength(500)] public TestSteps? TestSteps { get; set; } // Navigation property to TestSteps


    public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.NotRun;

    // Version of the test case execution, starting at 1 and incrementing
    public int Version { get; set; }

    // Indicates whether this version of the test case execution is active or archived
    public bool IsActive { get; set; } = true;

    public TimeSpan Duration { get; set; } // Duration of the test case execution

    [StringLength(500)] public string ExecutionNotes { get; set; } = string.Empty; // Notes for the test case execution

    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    // Actual result from the test step execution
    public string? ActualResult { get; set; }


    [StringLength(500)] public string? CreatedBy { get; set; }

    [StringLength(500)] public string? ModifiedBy { get; set; }


    public TestStepsExecutionFile? TestStepsExecutionFile { get; set; }
}