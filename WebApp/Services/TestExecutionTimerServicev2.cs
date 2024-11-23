namespace WebApp.Services;

public class TestExecutionTimerServicev2 : IDisposable
{
    private TimeSpan _timeElapsed;
    private DateTime _startExecutionTime;
    private DateTime? _pauseTime; // Time when paused
    private readonly PeriodicTimer _periodicTimer = new PeriodicTimer(TimeSpan.FromSeconds(1)); // 1 second interval
    private bool _executionInProgress;
    private bool _isPaused; // Track whether execution is paused
    public TimeSpan GetElapsedTime() => _timeElapsed;

    public bool IsExecutionInProgress() => _executionInProgress;

    public bool IsPaused() => _isPaused; // Check if paused

    // Event to notify when the elapsed time is updated, including testExecutionId
    public event Action<TimeSpan, int>? OnTimeUpdated;

    public event Action? OnExecutionStateChanged; // Event to notify state changes

    private int? _currentTestExecutionId; // Store the current test execution ID

    public async Task StartExecutionAndTrackTime(int testExecutionId)
    {
        if (_executionInProgress) return; // Prevent starting if already in progress

        _executionInProgress = true;
        _isPaused = false; // Reset paused state
        _startExecutionTime = DateTime.UtcNow;
        _timeElapsed = TimeSpan.Zero;
        _currentTestExecutionId = testExecutionId;
        
        OnExecutionStateChanged?.Invoke();

        // Start the timer loop
        await RunTimerLoop();
    }

    public async Task SuspendExecution(int testExecutionId)
    {
        if (!_executionInProgress) return; // Prevent suspending if not running or already paused

        _isPaused = true;
        _executionInProgress = false;
        _currentTestExecutionId = testExecutionId;

        OnExecutionStateChanged?.Invoke(); // Notify state change
        await Task.CompletedTask;
    }

    public async Task PauseExecution(int testExecutionId)
    {
        if (!_executionInProgress || _isPaused) return; // Prevent pausing if not running or already paused

        _isPaused = true;
        _pauseTime = DateTime.UtcNow;
        OnExecutionStateChanged?.Invoke(); // Notify state change
        await Task.CompletedTask;
    }

    public async Task ResumeExecution(int testExecutionId)
    {
        if (!_executionInProgress || !_isPaused) return; // Prevent resuming if not paused

        _isPaused = false;

        // Adjust the start time to account for the pause duration
        if (_pauseTime.HasValue)
        {
            var pauseDuration = DateTime.UtcNow - _pauseTime.Value;
            _startExecutionTime = _startExecutionTime.Add(pauseDuration); // Adjust start time
        }

        OnExecutionStateChanged?.Invoke();
        await RunTimerLoop();
    }

    public async Task ResumeSuspendedExecution(int testExecutionId, TimeSpan savedElapsedTime)
    {
        // Only resume if execution was suspended
        if (_executionInProgress) return;

        _isPaused = false;
        _executionInProgress = true;

        // Set the elapsed time to the previously saved time
        _timeElapsed = savedElapsedTime;

        // Calculate start time by subtracting saved elapsed time from current time
        _startExecutionTime = DateTime.UtcNow - _timeElapsed;
        _currentTestExecutionId = testExecutionId;

        OnExecutionStateChanged?.Invoke();
        await RunTimerLoop();
    }

    private async Task RunTimerLoop()
    {
        while (_executionInProgress && !_isPaused)
        {
            // Wait for the next tick
            if (!await _periodicTimer.WaitForNextTickAsync()) break;

            // Update the elapsed time and trigger the event
            UpdateElapsedTime();
        }
    }

    private void UpdateElapsedTime()
    {
        if (_isPaused || !_executionInProgress) return; // Do nothing if paused or not running

        _timeElapsed = DateTime.UtcNow - _startExecutionTime;

        if (_currentTestExecutionId.HasValue)
        {
            OnTimeUpdated?.Invoke(_timeElapsed, _currentTestExecutionId.Value);
        }
    }

    public async Task EndExecution(int testExecutionId)
    {
        if (!_executionInProgress) return;

        _executionInProgress = false;
        _currentTestExecutionId = testExecutionId;
        OnExecutionStateChanged?.Invoke();
        await Task.CompletedTask;
    }

    public async Task CancelExecution(int testExecutionId)
    {
        if (!_executionInProgress) return;

        _executionInProgress = false;
        _currentTestExecutionId = null;
        OnExecutionStateChanged?.Invoke();
        await Task.CompletedTask;
    }



    public void Dispose()
    {
        _periodicTimer.Dispose();
    }
}
