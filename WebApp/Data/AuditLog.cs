namespace WebApp.Data;

public class AuditLog
{
    public int Id { get; set; }

    // Required fields
    public string EntityName { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Action { get; set; } = null!;

    // Nullable fields
    public string? EntityId { get; set; } // PK as string, may be null
    public string? BeforeData { get; set; } // May be null for new entities
    public string? AfterData { get; set; } // May be null for deleted entities

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}