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
        var logger = LoggerService.Logger;

        ProjectId = projectId;
        logger.LogInformation($"SetProjectId {ProjectId}");
    }

    // Clear the project ID by setting it to a default value (like 0).
    public void ClearProjectId()
    {
        var logger = LoggerService.Logger;
        ProjectId = 0; // 0 can represent "no project" or "invalid project" as needed.
        logger.LogInformation($"ClearProjectId {ProjectId}");
    }

    public Task<int> GetProjectIdAsync()
    {
        var logger = LoggerService.Logger;
        if (ProjectId == 0) throw new InvalidOperationException("ProjectId is not set.");
        logger.LogInformation($"GetProjectId {ProjectId}");
        return Task.FromResult(ProjectId);
    }
}