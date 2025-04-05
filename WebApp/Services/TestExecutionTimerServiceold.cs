using WebApp.Data;

namespace WebApp.Services;

public class TestExecutionTimerServiceold : IDisposable
{
    private TimeSpan _timeElapsed;
    private DateTime _startExecutionTime;
    private DateTime? _pauseTime; // Time when paused
    private readonly PeriodicTimer _periodicTimer = new(TimeSpan.FromSeconds(1)); // 1 second interval
    private bool _executionInProgress;
    private bool _isPaused; // Track whether execution is paused

    public TimeSpan GetElapsedTime()
    {
        return _timeElapsed;
    }

    public bool IsExecutionInProgress()
    {
        return _executionInProgress;
    }

    public bool IsPaused()
    {
        return _isPaused;
        // Check if paused
    }

    // Event to notify when the elapsed time is updated, including testExecutionId
    public event Action<TimeSpan, int>? OnTimeUpdated;

    public event Action? OnExecutionStateChanged; // Event to notify state changes

    // private int? _currentTestExecutionId; // Store the current test execution ID

    private TestExecution? _testExecution;

    /*public async Task StartExecutionAndTrackTime(TestExecution testExecution)
    {
        if (_executionInProgress) return; // Prevent starting if already in progress

        _executionInProgress = true;
        _isPaused = false; // Reset paused state
        _startExecutionTime = DateTime.UtcNow;
        _timeElapsed = TimeSpan.Zero;
        _testExecution = testExecution;

        OnExecutionStateChanged?.Invoke();
        // Start the timer loop
        await RunTimerLoop();
    }

    public async Task SuspendExecution(TestExecution testExecution)
    {
        if (!_executionInProgress) return; // Prevent suspending if not running or already paused

        _isPaused = true;
        _executionInProgress = false;
        _testExecution = testExecution;

        OnExecutionStateChanged?.Invoke(); // Notify state change
        await Task.CompletedTask;
    }

    public async Task PauseExecution(TestExecution testExecution)
    {
        if (!_executionInProgress || _isPaused) return; // Prevent pausing if not running or already paused

        _isPaused = true;
        _pauseTime = DateTime.UtcNow;
        _testExecution = testExecution;
        OnExecutionStateChanged?.Invoke(); // Notify state change
        await Task.CompletedTask;
    }

    public async Task ResumeExecution(TestExecution testExecution)
    {
        if (!_executionInProgress || !_isPaused) return; // Prevent resuming if not paused

        _isPaused = false;
        _testExecution = testExecution;

        // Adjust the start time to account for the pause duration
        if (_pauseTime.HasValue)
        {
            var pauseDuration = DateTime.UtcNow - _pauseTime.Value;
            _startExecutionTime = _startExecutionTime.Add(pauseDuration); // Adjust start time
        }

        OnExecutionStateChanged?.Invoke();
        await RunTimerLoop();
    }

    public async Task ResumeSuspendedExecution(TestExecution testExecution)
    {
        // Only resume if execution was suspended
        if (_executionInProgress) return;

        _isPaused = false;
        _executionInProgress = true;

        // Set the elapsed time to the Duration of the testExecution
        _timeElapsed = testExecution.Duration;

        // Calculate start time by subtracting saved elapsed time from current time
        _startExecutionTime = DateTime.UtcNow - _timeElapsed;
        _testExecution = testExecution;

        OnExecutionStateChanged?.Invoke();
        await RunTimerLoop();
    }*/

    /*private async Task RunTimerLoop()
    {
        while (_executionInProgress && !_isPaused)
        {
            // Wait for the next tick
            if (!await _periodicTimer.WaitForNextTickAsync()) break;

            // Update the elapsed time and trigger the event
            UpdateElapsedTime();
        }
    }*/

    /*private void UpdateElapsedTime()
    {
        if (_isPaused || !_executionInProgress) return; // Do nothing if paused or not running

        _timeElapsed = DateTime.UtcNow - _startExecutionTime;

        if (_testExecution != null)
        {
            OnTimeUpdated?.Invoke(_timeElapsed, _testExecution.Id);
        }
    }

    public async Task EndExecution(TestExecution testExecution)
    {
        if (!_executionInProgress) return;

        _executionInProgress = false;
        _testExecution = testExecution;
        OnExecutionStateChanged?.Invoke();
        await Task.CompletedTask;
    }

    public async Task CancelExecution(TestExecution testExecution)
    {
        if (!_executionInProgress) return;

        _executionInProgress = false;
        _testExecution = null;
        OnExecutionStateChanged?.Invoke();
        await Task.CompletedTask;
    }*/


    public void Dispose()
    {
        _periodicTimer.Dispose();
    }
}