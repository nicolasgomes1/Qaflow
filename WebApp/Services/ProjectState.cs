namespace WebApp.Services;

using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Threading;
using System.Threading.Tasks;

public class ProjectState
{
    private readonly IJSRuntime _jsRuntime;
    private int? _currentProject; // Cached project ID
    private const string StorageKey = "CurrentProject"; // Key for local storage

    public ProjectState(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    // Retrieve the current project ID from local storage
    public async Task<string> GetCurrentProjectAsync()
    {
        if (_currentProject.HasValue)
            return _currentProject.Value.ToString(); // Return cached value if available

        var storedValue = await _jsRuntime.InvokeAsync<string>("window.localStorage.getItem", StorageKey);
        if (int.TryParse(storedValue, out var projectId))
        {
            _currentProject = projectId;
            return projectId.ToString();
        }

        return 0.ToString(); // Default to 0 if nothing is stored
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