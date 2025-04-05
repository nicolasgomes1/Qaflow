using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class TestExecutionModelv2(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
{
    //single selection
    public int SelectedTestPlan { get; set; } = -1;


    public async Task LoadTestPlanForTestExecution(int testExecution, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var execution =
            await db.TestExecution.FirstOrDefaultAsync(te => te.Id == testExecution && te.ProjectsId == projectId);

        if (execution is null) return;

        SelectedTestPlan = execution.TestPlanId;
    }

    public async Task<TestExecution> AddTestExecution(TestExecution testExecution, int projectId)
    {
        var execution = new TestExecution();

        await using var db = await dbContextFactory.CreateDbContextAsync();

        testExecution.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testExecution.ProjectsId = projectId;

        if (testExecution.WorkflowStatus == WorkflowStatus.Completed)
            testExecution.ArchivedStatus = ArchivedStatus.Archived;

        var testPlan = await db.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .ThenInclude(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tp => tp.Id == SelectedTestPlan);

        if (testPlan is null) return testExecution;

        testExecution.TestPlanId = testPlan.Id;

        foreach (var testCase in testPlan.LinkedTestCases)
        {
            var testCaseExecution = new TestCaseExecution
            {
                TestExecution = testExecution,
                TestCaseId = testCase.Id,
                ExecutionStatus = execution.ExecutionStatus,
                Version = execution.Version,
                CreatedBy = testExecution.CreatedBy,
                CreatedAt = DateTime.UtcNow,
                ModifiedAt = DateTime.UtcNow,
                LinkedTestStepsExecution = new List<TestStepsExecution>()
            };

            foreach (var testStep in testCase.TestSteps)
            {
                var testStepExecution = new TestStepsExecution
                {
                    TestCaseExecution = testCaseExecution,
                    TestStepsId = testStep.Id,
                    ExecutionStatus = ExecutionStatus.NotRun,
                    Version = execution.Version,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = testExecution.CreatedBy
                };

                testCaseExecution.LinkedTestStepsExecution.Add(testStepExecution);
            }

            testExecution.LinkedTestCaseExecutions.Add(testCaseExecution);
        }

        db.TestExecution.Add(testExecution);
        await db.SaveChangesAsync();

        return testExecution;
    }

    public async Task UpdateTestExecution(TestExecution testExecution, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var existingTestExecution = await db.TestExecution
            .Where(p => p.ProjectsId == projectId)
            .Include(te => te.LinkedTestCaseExecutions)
            .ThenInclude(tce => tce.LinkedTestStepsExecution)
            .FirstOrDefaultAsync(te => te.Id == testExecution.Id);

        if (existingTestExecution == null)
            throw new InvalidOperationException("TestExecution not found.");

        existingTestExecution.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        existingTestExecution.ModifiedAt = DateTime.UtcNow;

        if (existingTestExecution.TestPlanId != SelectedTestPlan)
        {
            var newTestPlan = await db.TestPlans
                .Include(tp => tp.LinkedTestCases)
                .ThenInclude(tc => tc.TestSteps)
                .FirstOrDefaultAsync(tp => tp.Id == SelectedTestPlan);

            if (newTestPlan == null)
                throw new InvalidOperationException($"TestPlan with ID {testExecution.TestPlanId} not found.");

            existingTestExecution.TestPlanId = SelectedTestPlan;
            existingTestExecution.LinkedTestCaseExecutions.Clear();

            foreach (var testCase in newTestPlan.LinkedTestCases)
            {
                var testCaseExecution = new TestCaseExecution
                {
                    TestExecution = existingTestExecution,
                    TestCaseId = testCase.Id,
                    ExecutionStatus = existingTestExecution.ExecutionStatus,
                    Version = existingTestExecution.Version,
                    CreatedBy = existingTestExecution.ModifiedBy,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                    LinkedTestStepsExecution = testCase.TestSteps.Select(testStep => new TestStepsExecution
                    {
                        TestCaseExecution = null, // Will be set when added
                        TestStepsId = testStep.Id,
                        ExecutionStatus = ExecutionStatus.NotRun,
                        Version = existingTestExecution.Version,
                        CreatedAt = DateTime.UtcNow,
                        CreatedBy = existingTestExecution.ModifiedBy
                    }).ToList()
                };

                foreach (var testStepExecution in testCaseExecution.LinkedTestStepsExecution)
                    testStepExecution.TestCaseExecution = testCaseExecution;

                existingTestExecution.LinkedTestCaseExecutions.Add(testCaseExecution);
            }
        }

        await db.SaveChangesAsync();
    }

    /// <summary>
    /// Creates a new Test Execution with the provided details and associates it with the given project.
    /// </summary>
    /// <param name="testExecution">The Test Execution object containing the necessary details for the new execution.</param>
    /// <param name="projectId">The ID of the project to associate with the new Test Execution.</param>
    /// <returns>A new <see cref="TestExecution"/> object initialized with the specified details.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the associated Test Plan specified in the provided Test Execution cannot be found.
    /// </exception>
    public async Task<TestExecution?> CreateNewExecutionAsync(TestExecution testExecution, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlan = await db.TestPlans
                           .Include(tp => tp.LinkedTestCases)
                           .ThenInclude(tc => tc.TestSteps)
                           .FirstOrDefaultAsync(tp => tp.Id == testExecution.TestPlanId)
                       ?? throw new InvalidOperationException("TestPlan not found.");

        var createdBy = (await userService.GetCurrentUserInfoAsync()).UserName;

        var newExecution = new TestExecution
        {
            Name = testExecution.Name,
            Description = testExecution.Description,
            ExecutionStatus = ExecutionStatus.NotRun,
            Version = testExecution.Version + 1,
            CreatedAt = DateTime.UtcNow,
            TestPlanId = testPlan.Id,
            EstimatedTime = testExecution.EstimatedTime,
            AssignedTo = testExecution.AssignedTo,
            CreatedBy = createdBy,
            ProjectsId = projectId,
            Priority = testExecution.Priority
        };

        foreach (var testCase in testPlan.LinkedTestCases)
        {
            var newTestCaseExecution = new TestCaseExecution
            {
                TestExecution = newExecution,
                TestCaseId = testCase.Id,
                ExecutionStatus = ExecutionStatus.NotRun,
                Version = newExecution.Version,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                LinkedTestStepsExecution = new List<TestStepsExecution>()
            };

            foreach (var testStep in testCase.TestSteps)
            {
                var newTestStepsExecution = new TestStepsExecution
                {
                    TestCaseExecution = newTestCaseExecution, // 🔧 Important!
                    TestStepsId = testStep.Id,
                    ExecutionStatus = ExecutionStatus.NotRun,
                    Version = newExecution.Version,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                newTestCaseExecution.LinkedTestStepsExecution.Add(newTestStepsExecution);
            }

            newExecution.LinkedTestCaseExecutions.Add(newTestCaseExecution);
        }

        db.TestExecution.Add(newExecution);
        await db.SaveChangesAsync();

        return newExecution;
    }


    /// <summary>
    /// Duplicates an existing Test Execution by creating a new instance with updated details, associating it with the specified project.
    /// </summary>
    /// <param name="testExecutionId">The ID of the Test Execution to duplicate.</param>
    /// <param name="projectId">The ID of the project to associate with the duplicated Test Execution.</param>
    /// <returns>A new <see cref="TestExecution"/> instance containing the duplicated details.</returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when the specified Test Execution to duplicate cannot be found.
    /// </exception>
    public async Task<TestExecution> DuplicateExecutionAsync(int testExecutionId, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var originalExecution = await db.TestExecution
                                    .Include(te => te.LinkedTestCaseExecutions)
                                    .ThenInclude(tce => tce.LinkedTestStepsExecution)
                                    .FirstOrDefaultAsync(te => te.Id == testExecutionId)
                                ?? throw new InvalidOperationException("Original TestExecution not found.");

        var createdBy = (await userService.GetCurrentUserInfoAsync()).UserName;

        var newExecution = new TestExecution
        {
            Name = originalExecution.Name + " (Copy)",
            Description = originalExecution.Description,
            ExecutionStatus = ExecutionStatus.NotRun,
            // Version = originalExecution.Version + 1,
            Version = 0,
            CreatedAt = DateTime.UtcNow,
            TestPlanId = originalExecution.TestPlanId,
            EstimatedTime = originalExecution.EstimatedTime,
            AssignedTo = originalExecution.AssignedTo,
            CreatedBy = createdBy,
            ProjectsId = projectId,
            Priority = originalExecution.Priority,
            LinkedTestCaseExecutions = new List<TestCaseExecution>()
        };

        foreach (var originalTce in originalExecution.LinkedTestCaseExecutions)
        {
            var newTce = new TestCaseExecution
            {
                TestExecution = newExecution,
                TestCaseId = originalTce.TestCaseId,
                ExecutionStatus = ExecutionStatus.NotRun,
                Version = newExecution.Version,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                LinkedTestStepsExecution = new List<TestStepsExecution>()
            };

            foreach (var originalStep in originalTce.LinkedTestStepsExecution)
            {
                var newStep = new TestStepsExecution
                {
                    TestCaseExecution = newTce,
                    TestStepsId = originalStep.TestStepsId,
                    ExecutionStatus = ExecutionStatus.NotRun,
                    Version = newExecution.Version,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                };

                newTce.LinkedTestStepsExecution.Add(newStep);
            }

            newExecution.LinkedTestCaseExecutions.Add(newTce);
        }

        db.TestExecution.Add(newExecution);
        await db.SaveChangesAsync();

        return newExecution;
    }
}