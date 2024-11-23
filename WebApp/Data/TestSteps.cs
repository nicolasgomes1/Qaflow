using System.ComponentModel.DataAnnotations;
using WebApp.Data.enums;

namespace WebApp.Data;

public class TestSteps
{
    public int Id { get; set; }

    public int Number { get; set; }

    [Required] [StringLength(500)] public string? Description { get; set; }

    [Required] [StringLength(500)] public string? ExpectedResult { get; set; }

    public int TestCaseId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;

    public TestCases TestCases { get; set; } = null!;


    public ArchivedStatus ArchivedStatus { get; set; } = ArchivedStatus.Active;

    [StringLength(500)] public string? CreatedBy { get; set; }

    [StringLength(500)] public string? ModifiedBy { get; set; }
}