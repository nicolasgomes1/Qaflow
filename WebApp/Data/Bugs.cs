using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Bugs : BaseEntity
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(150)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public Severity Severity { get; set; }

    public BugStatus BugStatus { get; set; }
    
    public int? TestCaseExecutionId { get; set; } // Value can be null value to be filled when running a test execution
    
    public TestCaseExecution? TestCaseExecutions { get; set; }

    public int ProjectsId { get; set; }

    public Projects? Projects { get; init; }

    public ICollection<BugsFiles>? BugFiles { get; set; }

    public ICollection<BugsComments>? BugComments { get; set; }
    
    public WorkflowStatus WorkflowStatus { get; set; }
    
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;
}