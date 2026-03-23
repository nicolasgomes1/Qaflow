namespace WebApp.Services;

public class TestCaseExecutionTimerService
{
    private readonly HashSet<int> _pausedTestCases = new(); // Track paused test cases
    private readonly Dictionary<int, TimeSpan> _testCaseElapsedTimes = new(); // Track elapsed time for each test case
    private readonly Dictionary<int, DateTime> _testCaseStartTimes = new();


    // Dictionary to hold test case ID and their paused state
    private readonly Dictionary<int, bool> testCasePausedStates = new();
    private readonly Dictionary<int, bool> testCasesRunningStates = new();
    private Timer? _testCaseTimer;
    private bool _timerIsRunning;
    private Dictionary<int, bool> testCasesStartedStates = new();


    public bool IsTestCaseRunning(int testCaseId)
    {
        // Return the running state if it exists; otherwise, return false
        return testCasesRunningStates.TryGetValue(testCaseId, out var isRunning) && isRunning;
    }

    public void SetTestCaseRunning(int testCaseId, bool isRunning)
    {
        // Update the running state in the dictionary
        testCasesRunningStates[testCaseId] = isRunning;
    }

    // Method to check if a test case is paused
    public bool IsTestCasePaused(int testCaseId)
    {
        // Return the paused state if it exists; otherwise, return false
        return testCasePausedStates.TryGetValue(testCaseId, out var isPaused) && isPaused;
    }

    // Method to set the paused state of a test case
    public void SetTestCasePaused(int testCaseId, bool isPaused)
    {
        // Update the paused state in the dictionary
        testCasePausedStates[testCaseId] = isPaused;
    }


    public event Action<int, TimeSpan>? OnTestCaseDurationUpdated;

    public async Task StartTestCaseExecution(int testCaseId)
    {
        if (_testCaseStartTimes.ContainsKey(testCaseId)) return;

        _testCaseStartTimes[testCaseId] = DateTime.UtcNow;
        _testCaseElapsedTimes[testCaseId] = TimeSpan.Zero;

        if (!_timerIsRunning)
        {
            _testCaseTimer = new Timer(UpdateTestCaseDurations, null, 0, 1000); // Update every second
            _timerIsRunning = true;
        }

        SetTestCaseRunning(testCaseId, true);


        await Task.CompletedTask;
    }

    public async Task PauseTestCaseExecution(int testCaseId)
    {
        if (!_testCaseStartTimes.ContainsKey(testCaseId)) return;

        _testCaseElapsedTimes[testCaseId] += DateTime.UtcNow - _testCaseStartTimes[testCaseId];
        _pausedTestCases.Add(testCaseId); // Mark as paused
        SetTestCasePaused(testCaseId, true);
        SetTestCaseRunning(testCaseId, false);

        await Task.CompletedTask;
    }

    public async Task ResumeTestCaseExecution(int testCaseId)
    {
        SetTestCasePaused(testCaseId, false);
        SetTestCaseRunning(testCaseId, true);

        if (!_testCaseStartTimes.ContainsKey(testCaseId)) return;

        _testCaseStartTimes[testCaseId] = DateTime.UtcNow; // Reset start time to now
        _pausedTestCases.Remove(testCaseId); // Remove from paused state

        await Task.CompletedTask;
    }

    public async Task EndTestCaseExecution(int testCaseId)
    {
        if (!_testCaseStartTimes.TryGetValue(testCaseId, out var startTime)) return;

        var elapsed = _pausedTestCases.Contains(testCaseId)
            ? _testCaseElapsedTimes[testCaseId]
            : _testCaseElapsedTimes[testCaseId] + (DateTime.UtcNow - startTime);

        OnTestCaseDurationUpdated?.Invoke(testCaseId, elapsed);

        _testCaseStartTimes.Remove(testCaseId);
        _testCaseElapsedTimes.Remove(testCaseId);
        _pausedTestCases.Remove(testCaseId);

        if (_testCaseStartTimes.Count == 0)
        {
            _testCaseTimer?.Dispose();
            _testCaseTimer = null;
            _timerIsRunning = false;
        }

        SetTestCaseRunning(testCaseId, false);
        await Task.CompletedTask;
    }

    private void UpdateTestCaseDurations(object? state)
    {
        foreach (var testCaseId in _testCaseStartTimes.Keys)
        {
            if (IsTestCasePaused(testCaseId)) continue; // Skip paused test cases

            var elapsed = _testCaseElapsedTimes[testCaseId] + (DateTime.UtcNow - _testCaseStartTimes[testCaseId]);
            OnTestCaseDurationUpdated?.Invoke(testCaseId, elapsed);
        }
    }
}