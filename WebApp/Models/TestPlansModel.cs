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

    public List<TestPlans> TestPlansList { get; private set; } = [];

    public List<int> SelectedTestCasesIds { get; set; } = [];


    /// <summary>
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
            .Where(p => p.TPProjectId == projectStateService.ProjectId)
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
    }


    /// <returns>A List of All Test Plans</returns>
    // public async Task GetallTestPlans()
    // {
    //     TestPlansList = await _dbContext.TestPlans.Where(tp => tp.TPProjectId == projectStateService.ProjectId).ToListAsync();
    // }

    /// <summary>
    /// Returns a list of all test plans within the project
    /// </summary>
    /// <returns></returns>
    public async Task<List<TestPlans>> GetallTestPlans()
    {
        return await _dbContext.TestPlans.Where(tp => tp.TPProjectId == projectStateService.ProjectId).ToListAsync();
    }
    
    public async Task<List<TestPlans>> GetCompletedTestPlans()
    {
        return await _dbContext.TestPlans.Where(tp => tp.TPProjectId == projectStateService.ProjectId && tp.WorkflowStatus == WorkflowStatus.Completed).ToListAsync();
    }


    public async Task<TestPlans> GetTestPlanByIdAsync(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans != null)
        {
            TestPlans = testPlans;
        }

        return TestPlans;
    }


    public async Task GetAssociatedTestCases(int testPlanId)
    {
        var testPlans = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");
        SelectedTestCasesIds = testPlans.TestCases.Select(tc => tc.Id).ToList();
    }

    public async Task CreateTestPlan(TestPlans testPlan)
    {
        testPlan.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testPlan.TPProjectId = projectStateService.ProjectId;
        testPlan.TestCases = new List<TestCases>();

        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await _dbContext.TestCases.FindAsync(testCaseId);
            if (testCase == null) throw new Exception("Test case not found");

            testPlan.TestCases.Add(testCase);
        }

        _dbContext.TestPlans.Add(testPlan);
        await _dbContext.SaveChangesAsync();
    }


    public async Task<TestPlans> CreateTestPlan1(TestPlans testPlan, List<IBrowserFile>? files)
    {
        testPlan.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testPlan.TPProjectId = projectStateService.ProjectId;
        testPlan.TestCases = new List<TestCases>();

        foreach (var testCaseId in SelectedTestCasesIds)
        {
            var testCase = await _dbContext.TestCases.FindAsync(testCaseId);
            if (testCase == null) throw new Exception("Test case not found");

            testPlan.TestCases.Add(testCase);
        }

        _dbContext.TestPlans.Add(testPlan);
        await _dbContext.SaveChangesAsync();

        if (files != null && files.Count != 0)
        {
            await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id);
        }

        return testPlan;
    }


    public async Task UpdateTestPlan()
    {
        TestPlans.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;


        var existingTestPlan = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == TestPlans.Id);

        if (existingTestPlan != null)
        {
            existingTestPlan.TestCases = await _dbContext.TestCases
                .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
                .ToListAsync();

            existingTestPlan.Name = TestPlans.Name;
            existingTestPlan.Description = TestPlans.Description;
            existingTestPlan.Priority = TestPlans.Priority;

            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task UpdateTestPlan1(TestPlans TestPlan)
    {
        // Set the ModifiedBy property to the current user's username
        TestPlan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        TestPlan.TPProjectId = projectStateService.ProjectId;


        // Retrieve the existing test plan to ensure it exists in the database
        var existingTestPlan = await _dbContext.TestPlans
            .Include(tp => tp.TestCases)
            .FirstOrDefaultAsync(tp => tp.Id == TestPlan.Id);

        if (existingTestPlan != null)
        {
            // Optionally update the test cases if needed
            existingTestPlan.TestCases = await _dbContext.TestCases
                .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
                .ToListAsync();

            // Update the TestPlan object in the context
            _dbContext.Update(TestPlan);

            // Save changes to the database
            await _dbContext.SaveChangesAsync();
        }
    }


    public async Task UpdateTestPlan2(TestPlans testPlan, List<IBrowserFile>? files)
    {
        _dbContext.Update(testPlan);
        testPlan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testPlan.TPProjectId = projectStateService.ProjectId;

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