using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestCaseExecution : BaseEntity
{
    public int Id { get; set; }

    // Foreign key reference to TestExecution
    public int TestExecutionId { get; set; }
    public TestExecution? TestExecution { get; set; } // Navigation property to TestExecution

    public int TestCaseId { get; set; }
    public TestCases? TestCases { get; set; } // Navigation property to TestCase

    public ExecutionStatus ExecutionStatus { get; set; } = ExecutionStatus.NotRun;

    // Version of the test case execution, starting at 1 and incrementing
    public int Version { get; set; }

    /// <summary>
    /// Indicates whether this version of the test case execution is active or archived
    /// </summary>
    public bool IsActive { get; set; } = true;

    public TimeSpan Duration { get; set; } // Duration of the test case execution

    public string ExecutionNotes { get; set; } = string.Empty; // Notes for the test case execution

    public ICollection<TestStepsExecution>? LinkedTestStepsExecution { get; set; } = [];
}