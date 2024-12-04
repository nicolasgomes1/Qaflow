using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

/// <summary>
/// Base Entity with common properties for all
/// CreatedAt, ModifiedAt, CreatedBy, ModifiedBy, ArchivedStatus
/// </summary>
public abstract class BaseEntity
{
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    
    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }
    
    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

}