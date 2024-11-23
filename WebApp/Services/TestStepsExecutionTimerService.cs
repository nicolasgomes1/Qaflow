namespace WebApp.Services;

public class TestStepsExecutionTimerService
{
    private readonly Dictionary<int, DateTime> _testStepStartTimes = new();
    private Timer? _testStepTimer;
    private bool _timerIsRunning;

    public event Action<int, TimeSpan>? OnTestStepDurationUpdated;  // Event to notify duration updates

    public async Task StartTestStepExecution(int testStepId)
    {
        if (!_testStepStartTimes.ContainsKey(testStepId))
        {
            _testStepStartTimes[testStepId] = DateTime.UtcNow;

            // Start the UI update timer if it's not running
            if (!_timerIsRunning)
            {
                _testStepTimer = new Timer(UpdateTestStepDurations, null, 0, 1000);  // Update every second
                _timerIsRunning = true;
            }
        }
        await Task.CompletedTask;
    }

    public async Task EndTestStepExecution(int testStepId)
    {
        if (_testStepStartTimes.TryGetValue(testStepId, out var startTime))
        {
            var elapsed = DateTime.UtcNow - startTime;

            // Notify subscribers about the elapsed time
            OnTestStepDurationUpdated?.Invoke(testStepId, elapsed);

            // Remove the test step start time from the tracking dictionary
            _testStepStartTimes.Remove(testStepId);

            // Stop the timer if no test steps are being timed
            if (_testStepStartTimes.Count == 0)
            {
                _testStepTimer?.DisposeAsync();
                _testStepTimer = null;
                _timerIsRunning = false;
            }
        }
        await Task.CompletedTask;

    }
    
    private void DisposeTimer()
    {
        if (_testStepTimer == null) return;
        _testStepTimer.Dispose();
        _testStepTimer = null;
    }
    
    public void Dispose()
    {
        DisposeTimer();
    }
    
    

    private void UpdateTestStepDurations(object? state)
    {
        foreach (var testStepId in _testStepStartTimes.Keys)
        {
            // Calculate elapsed time
            var elapsed = DateTime.UtcNow - _testStepStartTimes[testStepId];

            // Notify subscribers about the updated duration
            OnTestStepDurationUpdated?.Invoke(testStepId, elapsed);
        }
    }
}