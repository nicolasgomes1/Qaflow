using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class BugsFiles
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? FileName { get; init; }

    [Required] public byte[]? FileContent { get; init; }

    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;

    // Foreign key to the Requirement
    public int BugId { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;

    public int BfProjectId { get; init; }

    public Projects? Projects { get; init; }

    public Bugs? Bugs { get; set; }
}