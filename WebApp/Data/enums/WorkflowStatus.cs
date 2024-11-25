using System.ComponentModel.DataAnnotations;

namespace WebApp.Data.enums;

public enum WorkflowStatus
{
    [Display(Name = "New")] New,
    [Display(Name = "In Review")] InReview,
    
    /// <summary>
    /// when completed the item has been reviewed and approved with no further action required
    /// </summary>
    [Display(Name = "Completed")] Completed, 
    
    /// <summary>
    /// If necessary, the item can be reopened for further review or action
    /// </summary>
    [Display(Name = "Reopened")] Reopened,
}