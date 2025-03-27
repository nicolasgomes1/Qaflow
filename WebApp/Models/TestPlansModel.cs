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
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    public TestPlans TestPlans { get; set; } = new();

    public List<int> SelectedTestCasesIds { get; set; } = [];


    public async Task<(IEnumerable<TestPlans> TestPlans, IList<TestPlans> SelectedTestPlans)>
        DisplayTestPlansIndexPage1(int projectId)
    {
        var testplans = await _dbContext.TestPlans
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
        return await _dbContext.TestPlans
            .Where(tp => tp.ProjectsId == projectId && tp.WorkflowStatus == status).ToListAsync();
    }


    public async Task<TestPlans> GetTestPlanByIdAsync(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");

        return testPlans;
    }


    public async Task GetAssociatedTestCases(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");
        SelectedTestCasesIds = testPlans.LinkedTestCases.Select(tc => tc.Id).ToList();
    }

    public async Task<TestPlans> CreateTestPlanAsync(TestPlans testPlan, List<IBrowserFile>? files, int projectId)
    {
        // Fetch current user and project ID asynchronously
        var currentUserInfo = userService.GetCurrentUserInfoAsync().Result.UserName;

        // Set test plan properties
        testPlan.CreatedBy = currentUserInfo;
        testPlan.ProjectsId = projectId;
        testPlan.LinkedTestCases = new List<TestCases>();

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await _dbContext.TestCases.FindAsync(testCaseId);
            if (testCase == null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            testPlan.LinkedTestCases.Add(testCase);
        }

        // Save the test plan
        await _dbContext.TestPlans.AddAsync(testPlan);
        await _dbContext.SaveChangesAsync();

        // Save associated files if provided
        if (files?.Any() == true) await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id, projectId);

        return testPlan;
    }

    public async Task UpdateTestPlan2(int testPlanId, List<IBrowserFile>? files, int projectId)
    {
        //find testplan with testcases
        var testplan = await _dbContext.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);
        if (testplan == null) throw new Exception("Test plan not found");

        _dbContext.Update(testplan);
        testplan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testplan.ProjectsId = projectId;

        // Update test cases of testplan
        testplan.LinkedTestCases = await _dbContext.TestCases
            .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
            .ToListAsync();

        await _dbContext.SaveChangesAsync();

        if (files != null && files.Count != 0) await testPlansFilesModel.SaveFilesToDb(files, testPlanId, projectId);
    }
}