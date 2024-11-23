using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class GridSettings
{
    public int Id { get; set; }

    [MaxLength(50)] [Required] public string UserName { get; set; } = string.Empty;

    [MaxLength(50)] [Required] public string GridName { get; set; } = string.Empty;
    public bool IsCompactMode { get; set; }
    public bool IsVirtualizationEnabled { get; set; }
    public bool IsFilterEnabled { get; set; }
    public bool IsSortingEnabled { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }
}