using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class TestPlansFile
{
    public int Id { get; set; }

    [Required] public string? FileName { get; set; }

    [Required] public byte[]? FileContent { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public int TestPlanId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public int TPFProjectId { get; set; }

    public Projects? Projects { get; set; }

    public TestPlans? TestPlans { get; set; }
}