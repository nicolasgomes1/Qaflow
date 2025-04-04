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

    public async Task<TestExecution?> CreateNewExecutionAsync(TestExecution testExecution, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlan = await db.TestPlans
                           .Include(tp => tp.LinkedTestCases)
                           .ThenInclude(tc => tc.TestSteps)
                           .FirstOrDefaultAsync(tp => tp.Id == testExecution.TestPlanId)
                       ?? throw new InvalidOperationException("TestPlan not found.");

        var createdBy = userService.GetCurrentUserInfoAsync().Result.UserName;

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
                LinkedTestStepsExecution = testCase.TestSteps.Select(testStep => new TestStepsExecution
                {
                    TestStepsId = testStep.Id,
                    ExecutionStatus = ExecutionStatus.NotRun,
                    Version = newExecution.Version,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                }).ToList()
            };

            newExecution.LinkedTestCaseExecutions.Add(newTestCaseExecution);
        }

        db.TestExecution.Add(newExecution);
        await db.SaveChangesAsync();

        return newExecution;
    }
}