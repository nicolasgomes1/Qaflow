using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class BugsFiles
{
    public int Id { get; set; }

    [MaxLength(50)] [Required] public string? FileName { get; set; }

    [Required] public byte[]? FileContent { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Foreign key to the Requirement
    public int BugId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public int ProjectsId { get; set; }

    public Projects? Projects { get; set; }

    public Bugs? Bugs { get; set; }
}