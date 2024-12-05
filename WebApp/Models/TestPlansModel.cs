using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class TestPlansModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    ProjectStateService projectStateService,
    TestPlansFilesModel testPlansFilesModel)
{
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    public TestPlans TestPlans { get; set; } = new();

    public List<int> SelectedTestCasesIds { get; set; } = [];


    /*/// <summary>
    /// Ienumerable
    /// </summary>
    public IEnumerable<TestPlans>? testplans;

    /// <summary>
    /// Ilist for selecting the line on the grid
    /// </summary>
    public IList<TestPlans> selectedTestPlans = new List<TestPlans>();


    public async Task DisplayTestPlansIndexPage()
    {
        testplans = await _dbContext.TestPlans
            .Where(p => p.ProjectsId == projectStateService.ProjectId)
            .Include(tp => tp.TestCases).ToListAsync();

        if (testplans != null)
        {
            var selection = testplans.FirstOrDefault();
            if (selection == null)
            {
                return;
            }

            selectedTestPlans = new List<TestPlans> { selection };
        }
    }*/
    
    public async Task<(IEnumerable<TestPlans> TestPlans, IList<TestPlans> SelectedTestPlans)> DisplayTestPlansIndexPage1()
    {
        var testplans = await _dbContext.TestPlans
            .Include(r => r.TestCases)
            .Where(tc => tc.ProjectsId == projectStateService.GetProjectIdAsync().Result)
            .ToListAsync();

        var selectedTestPlans = new List<TestPlans>();
        var selection = testplans.FirstOrDefault();
        if (selection != null)
        {
            selectedTestPlans.Add(selection);
        }

        return (testplans, selectedTestPlans);
    }
    
    
    public async Task<List<TestPlans>> GetallTestPlansWithWorkflowStatus(WorkflowStatus status = WorkflowStatus.Completed)
    {
        return await _dbContext.TestPlans.Where(tp => tp.ProjectsId == projectStateService.ProjectId && tp.WorkflowStatus == status).ToListAsync();
    }
    

    public async Task<TestPlans> GetTestPlanByIdAsync(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");

        return testPlans;
    }


    public async Task GetAssociatedTestCases(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");
        SelectedTestCasesIds = testPlans.TestCases.Select(tc => tc.Id).ToList();
    }
    
    //TODO
    public async Task<TestPlans> CreateTestPlanAsync(TestPlans testPlan, List<IBrowserFile>? files)
    {
        // Fetch current user and project ID asynchronously
        var currentUserInfo = userService.GetCurrentUserInfoAsync().Result.UserName;
        var projectId = projectStateService.GetProjectIdAsync().Result;

        // Set test plan properties
        testPlan.CreatedBy = currentUserInfo;
        testPlan.ProjectsId = projectId;
        testPlan.TestCases = new List<TestCases>();

        // Fetch and add test cases asynchronously
        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await _dbContext.TestCases.FindAsync(testCaseId);
            if (testCase == null)
                throw new Exception($"Test case with ID {testCaseId} not found.");

            testPlan.TestCases.Add(testCase);
        }

        // Save the test plan
        await _dbContext.TestPlans.AddAsync(testPlan);
        await _dbContext.SaveChangesAsync();

        // Save associated files if provided
        if (files?.Any() == true)
        {
            await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id);
        }

        return testPlan;
    }
    
    public async Task UpdateTestPlan2(TestPlans testPlan, List<IBrowserFile>? files)
    {
        _dbContext.Update(testPlan);
        testPlan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testPlan.ProjectsId = projectStateService.ProjectId;

        // Retrieve the existing test plan to ensure it exists in the database
        var existingTestPlan = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlan.Id);

        if (existingTestPlan == null) throw new Exception("Test plan not found");

        existingTestPlan.TestCases = await _dbContext.TestCases
            .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
            .ToListAsync();

        await _dbContext.SaveChangesAsync();

        // If there are files, attempt to save them
        if (files != null && files.Count != 0)
        {
            await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id);
        }
    }
}