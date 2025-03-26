// ProjectStateService.cs

using Microsoft.AspNetCore.Components;

namespace WebApp.Services;

public class ProjectStateService
{
    // ProjectId is now non-nullable, default to 0 (or some other invalid sentinel value).
    public int ProjectId { get; private set; }

    /// <summary>
    /// Set the project ID.
    /// </summary>
    /// <param name="projectId"></param>
    public void SetProjectId(int projectId)
    {
        ProjectId = projectId;
    }
}