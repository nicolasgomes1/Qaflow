using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestPlans
{
    public int Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty; // Ensure Name is always set

    public string? Description { get; set; }
    public Priority Priority { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TestCases> TestCases { get; set; } = new List<TestCases>();

    // List of selected TestCase IDs (if needed)
    public List<int> SelectedTestCasesIds { get; set; } = new();

    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    public string? CreatedBy { get; set; }

    public string? ModifiedBy { get; set; }

    public int ProjectsId { get; set; }

    public Projects? Projects { get; set; }
    
    public WorkflowStatus WorkflowStatus { get; set; }
    
    [MaxLength(50)]
    public string AssignedTo { get; set; } = string.Empty;

    public ICollection<TestPlansFile> TestPlansFiles { get; set; } = new List<TestPlansFile>();
}