using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class QAflowSettings
{
    public int Id { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }
    
    public int ProjectsId { get; set; }
    public Projects Projects { get; init; } = null!;
    
    public QAflowOptionsSettings QAflowOptionsSettings { get; set; }
    public bool IsIntegrationEnabled { get; set; }
}