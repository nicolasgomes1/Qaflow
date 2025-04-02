using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class TestPlansModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    TestPlansFilesModel testPlansFilesModel)
{
    public List<int> SelectedTestCasesIds { get; set; } = [];


    public async Task<(IEnumerable<TestPlans> TestPlans, IList<TestPlans> SelectedTestPlans)>
        DisplayTestPlansIndexPage1(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testplans = await db.TestPlans
            .Include(r => r.LinkedTestCases)
            .Where(tc => tc.ProjectsId == projectId)
            .ToListAsync();

        var selectedTestPlans = new List<TestPlans>();
        var selection = testplans.FirstOrDefault();
        if (selection != null) selectedTestPlans.Add(selection);

        return (testplans, selectedTestPlans);
    }


    public async Task<List<TestPlans>> GetallTestPlansWithWorkflowStatus(WorkflowStatus status, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestPlans
            .Where(tp => tp.ProjectsId == projectId && tp.WorkflowStatus == status).ToListAsync();
    }


    public async Task<TestPlans> GetTestPlanByIdAsync(int testPlanId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlans = await db.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");

        return testPlans;
    }


    public async Task GetAssociatedTestCases(int testPlanId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlans = await db.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");
        SelectedTestCasesIds = testPlans.LinkedTestCases.Select(tc => tc.Id).ToList();
    }

    public async Task<TestPlans> CreateTestPlanAsync(TestPlans testPlan, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        // Fetch current user and project ID asynchronously
        var currentUserInfo = userService.GetCurrentUserInfoAsync().Result.UserName;

        // Set test plan properties
        testPlan.CreatedBy = currentUserInfo;
        testPlan.ProjectsId = projectId;
        testPlan.LinkedTestCases = new List<TestCases>();

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await db.TestCases.FindAsync(testCaseId);
            if (testCase == null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            testPlan.LinkedTestCases.Add(testCase);
        }

        // Save the test plan
        await db.TestPlans.AddAsync(testPlan);
        await db.SaveChangesAsync();

        // Save associated files if provided
        if (files?.Any() == true) await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id, projectId);

        return testPlan;
    }

    public async Task UpdateTestPlan2(int testPlanId, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        //find testplan with testcases
        var testplan = await db.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);
        if (testplan == null) throw new Exception("Test plan not found");

        db.Update(testplan);
        testplan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testplan.ProjectsId = projectId;

        // Update test cases of testplan
        testplan.LinkedTestCases = await db.TestCases
            .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
            .ToListAsync();

        await db.SaveChangesAsync();

        if (files != null && files.Count != 0) await testPlansFilesModel.SaveFilesToDb(files, testPlanId, projectId);
    }

    public async Task<List<TestPlans>> GetTestPlansProjectTree(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestPlans
            .Where(p => p.ProjectsId == projectId)
            .Include(tp => tp.LinkedTestCases)
            .ToListAsync();
    }
}