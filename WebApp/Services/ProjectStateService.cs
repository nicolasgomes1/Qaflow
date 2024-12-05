// ProjectStateService.cs
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

    // Clear the project ID by setting it to a default value (like 0).
    public void ClearProjectId()
    {
        ProjectId = 0;  // 0 can represent "no project" or "invalid project" as needed.
    }

    // Return the project ID (throws if not set correctly).
    public int GetProjectId()
    {
        if (ProjectId == 0)
        {
            throw new InvalidOperationException("ProjectId is not set.");
        }

        return ProjectId;
    }
    
    
    public async Task<int> GetProjectIdAsync()
    {
        if (ProjectId == 0)
        {
            throw new InvalidOperationException("ProjectId is not set.");
        }

        return await Task.FromResult(ProjectId);
    }




}