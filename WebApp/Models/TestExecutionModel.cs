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

    public TestExecution TestExecution { get; set; } = new();

    private TestPlans? TestPlans { get; set; } = new();

    public int TestPlanId { get; set; } = -1;

    public List<TestExecution> TestExecutionList = [];

    public IEnumerable<TestExecution>? testexecution;
    public IList<TestExecution>? selectedExecution = new List<TestExecution>();

    /// <summary>
    /// Initialize TestExecution with TestPlan, TestCases, TestSteps, TestCaseExecutions, and TestStepsExecutions
    /// This is to be used when creazting a new TestExecution
    /// </summary>
    /// <returns></returns>
    /*
    public async Task<TestExecution> CreateTestExecution(TestExecution testExecution, int projectId)
    {
        // await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();


        testExecution.CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName;
        testExecution.ProjectsId = projectId;

        if (testExecution.WorkflowStatus == WorkflowStatus.Completed)
            testExecution.ArchivedStatus = ArchivedStatus.Archived;


        // var assignedUser = await _userManager.FindByIdAsync(testExecution.AssignedTo); // Get user by ID
        // if (assignedUser is { UserName: not null })
        // {
        //     testExecution.AssignedTo = assignedUser.UserName; // Save the user's username
        // }

        TestPlans = await _dbContext.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .ThenInclude(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tp => tp.Id == TestPlanId);

        if (TestPlans != null)
        {
            // Fetched TestPlans is not null, proceed with populating the TestExecution

            testExecution.TestPlan = TestPlans;

            foreach (var testCase in TestPlans.LinkedTestCases)
            {
                var testCaseExecution = new TestCaseExecution
                {
                    TestExecution = testExecution,
                    TestCaseId = testCase.Id,
                    ExecutionStatus = TestExecution.ExecutionStatus,
                    Version = TestExecution.Version,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow
                };

                // Fetched TestSteps is not null, proceed with populating the TestStepsExecutions

                foreach (var testStepsExecution in testCase.TestSteps.Select(testStep => new TestStepsExecution
                         {
                             TestCaseExecution = testCaseExecution,
                             TestStepsId = testStep.Id,
                             ExecutionStatus = ExecutionStatus.NotRun,
                             Version = TestExecution.Version,
                             CreatedAt = DateTime.UtcNow,
                             CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName
                         }))
                {
                    if (testCaseExecution.LinkedTestStepsExecution == null) throw new Exception("TestSteps is null");

                    testCaseExecution.LinkedTestStepsExecution.Add(testStepsExecution);
                }

                testExecution.LinkedTestCaseExecutions.Add(testCaseExecution);
            }

            _dbContext.TestExecution.Add(testExecution);
            await _dbContext.SaveChangesAsync();
        }

        return testExecution;
    }


    public async Task UpdateTestExecution(int testExecutionId, TestExecution testExecution)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        // Fetch the existing TestExecution and related data from the database
        var existingTestExecution = await db.TestExecution
            .Include(te => te.LinkedTestCaseExecutions)
            .ThenInclude(tce => tce.LinkedTestStepsExecution)
            .Include(te => te.TestPlan)
            .ThenInclude(tp => tp!.LinkedTestCases)
            .ThenInclude(tc => tc.TestSteps)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        if (existingTestExecution == null)
            throw new InvalidOperationException("TestExecution not found.");

        // Fetch the selected TestPlan
        var selectedTestPlan = await db.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .ThenInclude(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tp => tp.Id == TestPlanId);

        if (selectedTestPlan == null)
            throw new InvalidOperationException($"TestPlan with ID {TestPlanId} not found.");

        // Update common properties
        existingTestExecution.ModifiedBy = _userService.GetCurrentUserInfoAsync().Result.UserName;
        existingTestExecution.Name = testExecution.Name;
        existingTestExecution.Description = testExecution.Description;
        existingTestExecution.Priority = testExecution.Priority;
        existingTestExecution.ModifiedAt = DateTime.UtcNow;

        // Check if the TestPlan has changed
        var isTestPlanChanged = existingTestExecution.TestPlan?.Id != TestPlanId;

        if (isTestPlanChanged)
        {
            // Update TestPlan and clear existing TestCaseExecutions
            existingTestExecution.TestPlan = selectedTestPlan;
            existingTestExecution.LinkedTestCaseExecutions.Clear();

            foreach (var testCase in selectedTestPlan.LinkedTestCases)
            {
                var newTestCaseExecution = CreateTestCaseExecution(testCase, existingTestExecution);

                db.TestCaseExecution.Add(newTestCaseExecution);
                existingTestExecution.LinkedTestCaseExecutions.Add(newTestCaseExecution);
            }
        }
        else
        {
            // TestPlan didn't change, update existing TestCaseExecutions and TestSteps
            foreach (var testCaseExecution in existingTestExecution.LinkedTestCaseExecutions.ToList())
            {
                var testCase = existingTestExecution.TestPlan?.LinkedTestCases
                    .FirstOrDefault(tc => tc.Id == testCaseExecution.TestCaseId);

                if (testCase == null)
                {
                    db.TestCaseExecution.Remove(testCaseExecution);
                    continue;
                }

                UpdateTestCaseExecution(testCaseExecution, testCase, existingTestExecution);
            }
        }

        // Save changes
        await db.SaveChangesAsync();
    }
    */

    /*
    private void UpdateTestCaseExecution(TestCaseExecution existingTestCaseExecution, TestCases testCase,
        TestExecution existingTestExecution)
    {
        existingTestCaseExecution.ExecutionStatus = existingTestExecution.ExecutionStatus;
        existingTestCaseExecution.Version = existingTestExecution.Version;
        existingTestCaseExecution.ModifiedAt = DateTime.UtcNow;

        if (existingTestCaseExecution.LinkedTestStepsExecution != null)
        {
            var existingTestSteps = existingTestCaseExecution.LinkedTestStepsExecution.ToList();

            // Add new TestSteps or update existing ones
            foreach (var testStep in testCase.TestSteps)
            {
                var existingTestStepExecution = existingTestSteps
                    .FirstOrDefault(tse => tse.TestStepsId == testStep.Id);

                if (existingTestStepExecution == null)
                {
                    var newTestStepExecution = new TestStepsExecution
                    {
                        TestCaseExecution = existingTestCaseExecution,
                        TestStepsId = testStep.Id,
                        ExecutionStatus = ExecutionStatus.NotRun,
                        Version = existingTestExecution.Version,
                        CreatedAt = DateTime.UtcNow,
                        ModifiedAt = DateTime.UtcNow
                    };

                    existingTestCaseExecution.LinkedTestStepsExecution.Add(newTestStepExecution);
                }
                else
                {
                    existingTestStepExecution.ExecutionStatus = ExecutionStatus.NotRun;
                    existingTestStepExecution.Version = existingTestExecution.Version;
                    existingTestStepExecution.ModifiedAt = DateTime.UtcNow;
                }
            }

            // Remove any TestStepExecutions that no longer exist in the TestCase
            foreach (var testStepExecution in existingTestSteps.Where(ets =>
                         testCase.TestSteps.All(ts => ts.Id != ets.TestStepsId)))
                _dbContext.TestStepsExecution.Remove(testStepExecution);
        }
    }

    private TestCaseExecution CreateTestCaseExecution(TestCases testCase, TestExecution existingTestExecution)
    {
        var testCaseExecution = new TestCaseExecution
        {
            TestExecution = existingTestExecution,
            TestCaseId = testCase.Id,
            ExecutionStatus = ExecutionStatus.NotRun,
            Version = existingTestExecution.Version,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        };

        foreach (var testStep in testCase.TestSteps)
        {
            var newTestStepExecution = new TestStepsExecution
            {
                TestCaseExecution = testCaseExecution,
                TestStepsId = testStep.Id,
                ExecutionStatus = ExecutionStatus.NotRun,
                Version = existingTestExecution.Version,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow
            };

            if (testCaseExecution.LinkedTestStepsExecution != null)
                testCaseExecution.LinkedTestStepsExecution.Add(newTestStepExecution);
        }

        return testCaseExecution;
    }
    */


    /// <summary>
    /// Create a new TestExecution based on the current TestExecution
    /// </summary>
    /// <param name="testExecution"></param>
    /// <returns>The New Execution</returns>
    public async Task<TestExecution?> CreateNewExecutionAsync(TestExecution testExecution, int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        if (testExecution == null) throw new InvalidOperationException("TestExecution not found.");

        var testPlan = await db.TestPlans
                           .Include(tp => tp.LinkedTestCases)
                           .ThenInclude(tc => tc.TestSteps)
                           .FirstOrDefaultAsync(tp => tp.Id == testExecution.TestPlanId) ??
                       throw new InvalidOperationException("TestPlan not found.");


        // Create a new TestExecution with incremented version
        var newExecution = new TestExecution
        {
            Name = testExecution.Name,
            Description = testExecution.Description,
            ExecutionStatus = ExecutionStatus.NotRun,
            Version = testExecution.Version + 1,
            CreatedAt = DateTime.UtcNow,
            TestPlan = testPlan, // Set the TestPlan object directly
            EstimatedTime = testExecution.EstimatedTime,
            AssignedTo = testExecution.AssignedTo,
            CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName,
            ProjectsId = projectId,
            Priority = testExecution.Priority
        };

        // Populate TestCaseExecutions and TestStepsExecutions for the new TestExecution
        foreach (var testCase in testPlan.LinkedTestCases)
        {
            var newTestCaseExecution = new TestCaseExecution
            {
                TestExecution = newExecution,
                TestCaseId = testCase.Id,
                ExecutionStatus = ExecutionStatus.NotRun,
                Version = newExecution.Version,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName
            };

            // If there are TestSteps, add them to the TestCaseExecution
            foreach (var newTestStepsExecution in testCase.TestSteps.Select(testStep => new TestStepsExecution
                     {
                         TestCaseExecution = newTestCaseExecution,
                         TestStepsId = testStep.Id,
                         ExecutionStatus = ExecutionStatus.NotRun,
                         Version = newExecution.Version,
                         CreatedAt = DateTime.UtcNow,
                         CreatedBy = _userService.GetCurrentUserInfoAsync().Result.UserName
                     }))
            {
                if (newTestCaseExecution.LinkedTestStepsExecution == null) throw new Exception("TestSteps is null");

                newTestCaseExecution.LinkedTestStepsExecution.Add(newTestStepsExecution);
            }

            newExecution.LinkedTestCaseExecutions.Add(newTestCaseExecution);
        }

        // Add the new TestExecution to the DbContext
        db.TestExecution.Add(newExecution);

        // Save changes to the database
        await db.SaveChangesAsync();

        // Return the new execution object for navigation purposes
        return newExecution;
    }

    /// <summary>
    /// Load the test execution, including related entities (TestCaseExecution and TestStepExecution)
    /// </summary>
    /// <returns>The TestExecution object</returns>
    public async Task<TestExecution> GetTestExecutionData(int testExecutionId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var getTestExecutionData = await dbContext.TestExecution
            .Include(te => te.TestPlan)
            .Include(te => te.LinkedTestCaseExecutions)
            .ThenInclude(tce => tce.LinkedTestStepsExecution)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        if (getTestExecutionData is { TestPlan: not null }) TestPlanId = getTestExecutionData.TestPlan.Id;

        return getTestExecutionData ?? throw new InvalidOperationException("Test Execution Data was null here");
    }

    public async Task<List<TestExecution>> GetTestExecutionWithTestPlan()
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        return await dbContext.TestExecution
            .Include(te => te.TestPlan)
            .ToListAsync();
    }


    public async Task DisplayTestExecutionIndexPage(int projectId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        testexecution = await dbContext.TestExecution
            .Include(te => te.TestPlan) // Include the TestPlan navigation property
            .Where(te => te.ProjectsId == projectId)
            .ToListAsync();

        if (testexecution != null && testexecution.Any()) // Check if there are any elements
            selectedExecution = new List<TestExecution> { testexecution.First() };
        else
            selectedExecution = new List<TestExecution>(); // Handle the case where there are no test executions
    }

    public async Task DisplayTestExecutionIndexPageWithStatus(ExecutionStatus status, int projectId)
    {
        //  await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        testexecution = await _dbContext.TestExecution
            .Where(te => te.ExecutionStatus == status) // Compare the status directly
            .Include(te => te.TestPlan) // Include the TestPlan navigation property
            .Where(te => te.ProjectsId == projectId)
            .ToListAsync();

        if (testexecution != null && testexecution.Any()) // Check if there are any elements
            selectedExecution = new List<TestExecution> { testexecution.First() };
        else
            selectedExecution = new List<TestExecution>(); // Handle the case where there are no test executions
    }


    /// <summary>
    /// Gets the test cases for the TestExecution
    /// </summary>
    /// <param name="testExecutionId"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<List<TestCases>> GetTestCasesForTestExecution(int testExecutionId)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

        var testExecution = await dbContext.TestExecution
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
        await using var contextForFirstQuery = await _dbContextFactory.CreateDbContextAsync();

        // Load the current TestExecution based on Id
        var testExecution = await contextForFirstQuery.TestExecution
            .Include(te => te.TestPlan) // Include TestPlan to check for null
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        // Check if testExecution or its TestPlan is null
        if (testExecution?.TestPlan == null)
            throw new InvalidOperationException("Test Execution or Test Plan cannot be null");

        // Create a new DbContext for the second operation to avoid ongoing operation issues
        await using var contextForSecondQuery = await _dbContextFactory.CreateDbContextAsync();

        // Load past executions filtered directly from the database
        TestExecutionList = await contextForSecondQuery.TestExecution
            .Where(te => !te.IsActive
                         && te.TestPlan != null
                         && te.TestPlan.Id == testExecution.TestPlan.Id
                         && te.Name == testExecution.Name // Filter by the same name
                         && te.Id < testExecution.Id)
            .ToListAsync();

        return TestExecutionList;
    }


    public async Task<TestExecution> GetTestExecutionbyIdAsync(int testExecutionId)
    {
        await using var _dbContext = await _dbContextFactory.CreateDbContextAsync();

        var testexecution = await _dbContext.TestExecution
            .Include(te => te.TestPlan)
            .FirstOrDefaultAsync(te => te.Id == testExecutionId);

        if (testexecution != null)
            TestExecution = testexecution;

        return TestExecution;
    }

    public async Task<TestExecution> GetTestExecutionAsync(int testExecutionId)
    {
        return await _dbContext.TestExecution.FindAsync(testExecutionId) ??
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
        return await _dbContext.TestExecution.FindAsync(testExecutionId) ??
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
    public async Task<int> TestExecutionWIthPriority(Priority priority, int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.TestExecution
            .Where(te => te.Priority == priority)
            .Where(te => te.IsActive == false)
            .Where(te => te.ProjectsId == projectId)
            .CountAsync();
    }


    public async Task<int> GetTotalTestExecutionsAsync(int projectId)
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