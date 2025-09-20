using Microsoft.JSInterop;

namespace WebApp.Services;

public class ProjectState
{
    private const string StorageKey = "CurrentProject"; // Key for local storage
    private readonly IJSRuntime _jsRuntime;
    private int? _currentProject; // Cached project ID

    public ProjectState(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // Retrieve the current project ID from local storage
    public async Task<int> GetCurrentProjectAsync()
    {
        if (_currentProject.HasValue)
            return _currentProject.Value; // Return cached value if available

        var storedValue = await _jsRuntime.InvokeAsync<string>("window.localStorage.getItem", StorageKey);
        if (int.TryParse(storedValue, out var projectId))
        {
            _currentProject = projectId;
            return projectId;
        }

        return 0; // Default to 0 if nothing is stored
    }


    // Set and persist the current project ID
    public async Task SetCurrentProjectAsync(int projectId)
    {
        _currentProject = projectId;
        await _jsRuntime.InvokeVoidAsync("window.localStorage.setItem", StorageKey, projectId.ToString());
    }

    // Reset (clear) the current project
    public async Task ResetCurrentProjectAsync()
    {
        _currentProject = null;
        await _jsRuntime.InvokeVoidAsync("window.localStorage.removeItem", StorageKey);
    }
}