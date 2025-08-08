namespace WebApp.Data;

/// <summary>
/// Settings denoting a cycle a sprint a box time period
/// </summary>
public class Cycles : BaseEntity
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime StartDate { get; set; } = DateTime.UtcNow;

    public DateTime EndDate { get; set; } = DateTime.UtcNow;

    public int ProjectsId { get; set; }

    public Projects? Projects { get; init; }
}