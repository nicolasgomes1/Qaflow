using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class Bugs
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? Name { get; set; }

    [MaxLength(150)] public string? Description { get; set; }

    public Priority Priority { get; set; }

    public Severity Severity { get; set; }

    public BugStatus BugStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    public ArchivedStatus ArchivedStatus { get; set; }

    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }

    public int? TestCaseExecutionId { get; set; } // Value can be null value to be filled when running a test execution

    public int BProjectId { get; set; }

    public Projects? Projects { get; init; }

    public ICollection<BugsFiles>? BugFiles { get; set; }

    public ICollection<BugsComments>? BugComments { get; set; }
    
    public WorkflowStatus WorkflowStatus { get; set; }
    
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;
}