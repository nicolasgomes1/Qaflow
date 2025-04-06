using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class TestExecutionModel
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly UserService _userService;

    public TestExecutionModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
    {
        _dbContextFactory = dbContextFactory;
        _dbContext = _dbContextFactory.CreateDbContext();
        _userService = userService;
    }


    private int TestPlanId { get; set; } = -1;


    /// <summary>
    /// Load the test execution, including related entities (TestCaseExecution and TestStepExecution)
    /// </summary>
    /// <returns>The TestExecution object</returns>
    public async Task<TestExecution> GetTestExecutionData(int testExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var getTestExecutionData = await db.TestExecution
            .Include(te => te.TestPlan)
            .Include(te => te.LinkedTestCaseExecutions)
            .ThenInclude(tce => tce.LinkedTestStepsExecution)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        if (getTestExecutionData is { TestPlan: not null }) TestPlanId = getTestExecutionData.TestPlan.Id;

        return getTestExecutionData ?? throw new InvalidOperationException("Test Execution Data was null here");
    }

    public async Task<List<TestExecution>> GetTestExecutionWithTestPlan()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Include(te => te.TestPlan)
            .ToListAsync();
    }


    public async Task<List<TestExecution>> DisplayTestExecutionIndexPage(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var te = await db.TestExecution
            .Include(te => te.TestPlan) // Include the TestPlan navigation property
            .Where(te => te.ProjectsId == projectId)
            .ToListAsync();

        return te;
    }

    public async Task<List<TestExecution>> DisplayTestExecutionIndexPageWithStatus(ExecutionStatus status,
        int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var te = await db.TestExecution
            .Where(te => te.ExecutionStatus == status) // Compare the status directly
            .Include(te => te.TestPlan) // Include the TestPlan navigation property
            .Where(te => te.ProjectsId == projectId)
            .ToListAsync();

        return te;
    }


    /// <summary>
    /// Gets the test cases for the TestExecution
    /// </summary>
    /// <param name="testExecutionId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<List<TestCases>> GetTestCasesForTestExecution(int testExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var testExecution = await db.TestExecution
            .Include(te => te.TestPlan)
            .ThenInclude(tp => tp!.LinkedTestCases)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);
        if (testExecution == null) throw new InvalidOperationException("TestExecution not found.");

        var testPlan = testExecution.TestPlan;
        if (testPlan == null) throw new InvalidOperationException("TestPlan not found.");

        return testPlan.LinkedTestCases?.ToList() ?? new List<TestCases>();
    }

    public async Task<List<TestExecution>> GetPastTestExecutions(int testExecutionId)
    {
        // Create a new DbContext for the first operation
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        // Load the current TestExecution based on Id
        var testExecution = await db.TestExecution
            .Include(te => te.TestPlan) // Include TestPlan to check for null
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        // Check if testExecution or its TestPlan is null
        if (testExecution?.TestPlan == null)
            throw new InvalidOperationException("Test Execution or Test Plan cannot be null");

        // Create a new DbContext for the second operation to avoid ongoing operation issues
        await using var contextForSecondQuery = await _dbContextFactory.CreateDbContextAsync();

        // Load past executions filtered directly from the database
        var teList = await contextForSecondQuery.TestExecution
            .Where(te => !te.IsActive
                         && te.TestPlan != null
                         && te.TestPlan.Id == testExecution.TestPlan.Id
                         && te.Name == testExecution.Name // Filter by the same name
                         && te.Id < testExecution.Id)
            .ToListAsync();

        return teList;
    }


    public async Task<TestExecution> GetTestExecutionbyIdAsync(int testExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var te = await db.TestExecution
            .Include(te => te.TestPlan)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        if (te is null) throw new Exception("Test Execution not found");

        return te;
    }

    public async Task<TestExecution> GetTestExecutionAsync(int testExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution.FindAsync(testExecutionId) ??
               throw new Exception("Test Execution Id not found");
    }

    /// <summary>
    /// Returns the TestExecution object by Id
    /// </summary>
    /// <param name="testExecutionId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<TestExecution> GetTestExecutionByIdAsync(int testExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution.FindAsync(testExecutionId) ??
               throw new Exception("Test Execution Id not found");
    }


    /// <summary>
    /// Returns a List of TestCaseExecution for a given TestExecution With TestSteps
    /// </summary>
    /// <param name="testExecutionId"></param>
    /// <returns></returns>
    public async Task<List<TestCaseExecution>> GetTestCaseExecutionData(int testExecutionId)
    {
        var testExecution = await _dbContext.TestExecution.FindAsync(testExecutionId) ?? new TestExecution();

        return await _dbContext.TestCaseExecution
            .Include(tse => tse.LinkedTestStepsExecution)
            .Include(tc => tc.TestCases)
            .Where(tce => tce.TestExecutionId == testExecution.Id)
            .ToListAsync();
    }

    /// <summary>
    /// returns the list of TetsExecutions with the status of Completed and IsActive, so it was not yet executed
    /// </summary>
    /// <returns></returns>
    public async Task<List<TestExecution>> GetActiveTestExecutionsAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Include(te => te.TestPlan)
            .Where(te => te.IsActive && te.WorkflowStatus == WorkflowStatus.Completed)
            .Where(te => te.ProjectsId == projectId)
            .ToListAsync();
    }

    public async Task<TestCaseExecution?> GetTestCaseExecutionByIdAsync(int testCaseExecutionId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var testCaseExecution = await db.TestCaseExecution.FindAsync(testCaseExecutionId);

        return testCaseExecution;
    }

    public async Task<int> GetTestExecutionsReadyToExecuteAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();
        return await db.TestExecution
            .Where(te => te.IsActive && te.WorkflowStatus == WorkflowStatus.Completed)
            .Where(te => te.ProjectsId == projectId)
            .CountAsync();
    }

    /// <summary>
    /// Returns the count to Test Executions based on the priority
    /// </summary>
    /// <param name="priority"></param>
    /// <returns></returns>
    private async Task<int> TestExecutionWIthPriority(Priority priority, int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Where(te => te.Priority == priority)
            .Where(te => te.IsActive == false)
            .Where(te => te.ProjectsId == projectId)
            .CountAsync();
    }


    private async Task<int> GetTotalTestExecutionsAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Where(te => te.ProjectsId == projectId)
            .CountAsync();
    }


    public async Task<double> GetRationofTestExecutionsByPriorityAsync(Priority priority, int projectId)
    {
        var totalTestExecutions = await GetTotalTestExecutionsAsync(projectId);
        var testExecutionsWithPriority = await TestExecutionWIthPriority(priority, projectId);

        return totalTestExecutions == 0
            ? 0
            : Math.Round((double)testExecutionsWithPriority / totalTestExecutions * 100, 2);
    }

    public async Task<List<TestExecution>> GetActiveTestExecutionsAsyncWkf(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Where(te => te.IsActive == true && te.ProjectsId == projectId)
            .ToListAsync();
    }

    public async Task<List<TestExecution>> GetTestExecutionsAssignedToAll(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Where(te => te.IsActive == true && te.ProjectsId == projectId)
            .ToListAsync();
    }

    public async Task<List<TestExecution>> GetTestExecutionsAssignedToCurrentUser(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var currentUser = _userService.GetCurrentUserInfoAsync().Result.UserId;
        return await db.TestExecution.Where(rp => rp.ProjectsId == projectId && rp.AssignedTo == currentUser)
            .ToListAsync();
    }
}