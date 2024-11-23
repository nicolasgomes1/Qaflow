using System.Text.Json;
using Microsoft.JSInterop;
using Radzen;

namespace WebApp.Services;

public class DataGridSettingsService(IJSRuntime jsRuntime)
{
    private string? _currentGridId; // Track the current grid ID in use
    private DataGridSettings? _settings; // Hold the current grid settings in memory

    // Method to set the current grid's identifier (e.g., "requirements", "testcases")
    public void SetCurrentGridId(string gridId)
    {
        _currentGridId = gridId;
        _settings = null; // Reset settings to force reloading for the new grid
    }

    // Property to get or set the current grid settings
    public DataGridSettings? Settings
    {
        get => _settings;
        set
        {
            if (_settings == value) return;
            _settings = value;
            SaveSettingsAsync(_settings).ConfigureAwait(false); // Automatically save settings
        }
    }

    // Save DataGrid state for the current grid to local storage
    private async Task SaveSettingsAsync(DataGridSettings? settings)
    {
        if (settings != null && _currentGridId != null)
        {
            var storageKey = GetStorageKey(_currentGridId);
            var serializedSettings = JsonSerializer.Serialize(settings);
            await jsRuntime.InvokeVoidAsync("window.localStorage.setItem", storageKey, serializedSettings);
        }
    }

    // Load DataGrid state for the current grid from local storage
    public async Task<DataGridSettings?> LoadSettingsAsync()
    {
        if (_currentGridId == null) return null;

        var storageKey = GetStorageKey(_currentGridId);
        var savedSettings = await jsRuntime.InvokeAsync<string>("window.localStorage.getItem", storageKey);
        if (!string.IsNullOrEmpty(savedSettings))
        {
            _settings = JsonSerializer.Deserialize<DataGridSettings>(savedSettings);
        }

        return _settings; // Return the loaded settings for the current grid
    }

    // Clear the DataGrid settings for the current grid
    public async Task ClearSettingsAsync()
    {
        if (_currentGridId == null) return;

        var storageKey = GetStorageKey(_currentGridId);
        await jsRuntime.InvokeVoidAsync("window.localStorage.removeItem", storageKey);
        _settings = null; // Reset the local settings for the current grid
    }

    // Helper method to get the unique storage key for each grid based on the grid ID
    private static string GetStorageKey(string gridId)
    {
        return $"DataGridSettings_{gridId}";
    }
}