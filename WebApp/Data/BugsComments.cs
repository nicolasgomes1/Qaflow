using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class BugsComments
{
    public int Id { get; set; }

    [MaxLength(500)] public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public DateTime ModifiedAt { get; set; }

    [MaxLength(50)] public string? CreatedBy { get; set; }

    [MaxLength(50)] public string? ModifiedBy { get; set; }

    public int BugId { get; set; }

    public Bugs? Bugs { get; set; }
}