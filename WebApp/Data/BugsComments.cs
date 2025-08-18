using System.ComponentModel.DataAnnotations;

namespace WebApp.Data;

public class BugsComments : BaseEntity
{
    public int Id { get; set; }

    [MaxLength(500)] public string Comment { get; set; } = string.Empty;


    public int BugId { get; set; }

    public Bugs? Bugs { get; set; }
}