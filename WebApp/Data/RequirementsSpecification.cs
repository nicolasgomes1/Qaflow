using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace WebApp.Data;

public class RequirementsSpecification : BaseEntity
{
    public int Id { get; set; }

    [MaxLength(50)] [Required] public string Name { get; set; } = string.Empty;

    [MaxLength(100)] [Required] public string Description { get; set; } = string.Empty;

    public ICollection<Requirements> LinkedRequirements { get; set; } = [];

    public int ProjectsId { get; set; }

    public Projects? Projects { get; init; }

    [NotMapped] public string LinkedRequirementsName => string.Join(", ", LinkedRequirements.Select(t => t.Name));
}