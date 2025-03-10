// ProjectStateService.cs

using Microsoft.AspNetCore.Components;

namespace WebApp.Services;

public class ProjectStateService
{
    private readonly NavigationManager navigationManager;
    private readonly FormNotificationService formNotificationService;
    
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
    
    public  Task<int> GetProjectIdAsync()
    {
        if (ProjectId is not 0) return Task.FromResult(ProjectId);
        navigationManager.NavigateTo("/");
        formNotificationService.NotifyError("Returning to the Hub Home Page as no Project is selected");
        
        return  Task.FromResult(ProjectId);
    }
}