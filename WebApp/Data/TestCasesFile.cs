using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class TestCasesFile
{
    public int Id { get; init; }

    [MaxLength(50)] [Required] public string? FileName { get; init; }

    [Required] public byte[]? FileContent { get; init; }

    public DateTime UploadedAt { get; init; } = DateTime.UtcNow;


    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; init; } = DateTime.UtcNow;

    public int TcfProjectId { get; init; }

    public Projects? Projects { get; init; }

    public int TestCaseId { get; init; }
    public TestCases? TestCases { get; init; }
}