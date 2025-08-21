using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Radzen;
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
    public int SelectedCycleId { get; set; }

    public async Task<List<TestPlans>> DisplayTestPlansIndexPage(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlans = await db.TestPlans
            .Where(p => p.ProjectsId == projectId)
            .Include(tp => tp.LinkedTestCases).ToListAsync();

        return testPlans;
    }

    public async Task<List<TestPlans>> GetTestPlans(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestPlans.Where(rp => rp.ProjectsId == projectId).ToListAsync();
    }

    public async Task<List<TestPlans>> GetTestPlansAssignedToCurrentUser(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestPlans.Where(rp =>
                rp.ProjectsId == projectId && rp.AssignedTo == userService.GetCurrentUserInfoAsync().Result.UserId)
            .ToListAsync();
    }


    public async Task<List<TestPlans>> GetAllTestPlansWithWorkflowStatus(WorkflowStatus status, int projectId)
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
            .Include(tp => tp.Cycle) // Add this line to include the Cycle
            .FirstOrDefaultAsync(tp => tp.Id == testPlanId);

        if (testPlans == null) throw new ApplicationException("Test Plan not found");
        SelectedCycleId = testPlans.CycleId;
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
        testPlan.CycleId = SelectedCycleId;

        testPlan.LinkedTestCases = [];

        var testcases = await db.TestCases
            .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
            .ToListAsync();

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(testPlan);
        if (testcases.Count != SelectedTestCasesIds.Count)
            throw new Exception("Not all test cases were found.");


        testPlan.LinkedTestCases = testcases;


        // Save the test plan
        await db.TestPlans.AddAsync(testPlan);
        await db.SaveChangesAsync();

        // Save associated files if provided
        if (files != null && files.Count != 0) await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id, projectId);

        return testPlan;
    }


    public async Task UpdateTestPlan(TestPlans updatedTestPlan, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testPlan = await db.TestPlans
                           .Include(tp => tp.LinkedTestCases)
                           .Include(tp => tp.Cycle)
                           .FirstOrDefaultAsync(tp => tp.Id == updatedTestPlan.Id)
                       ?? throw new Exception("Test plan not found");

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(testPlan);

        testPlan.Name = updatedTestPlan.Name;
        testPlan.Description = updatedTestPlan.Description;
        testPlan.WorkflowStatus = updatedTestPlan.WorkflowStatus;
        testPlan.AssignedTo = updatedTestPlan.AssignedTo;
        testPlan.Priority = updatedTestPlan.Priority;
        testPlan.ProjectsId = projectId;
        testPlan.CycleId = SelectedCycleId;
        testPlan.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        testPlan.LinkedTestCases = await db.TestCases
            .Where(tc => SelectedTestCasesIds.Contains(tc.Id))
            .ToListAsync();

        db.TestPlans.Update(testPlan);
        await db.SaveChangesAsync();

        if (files is { Count: > 0 })
            await testPlansFilesModel.SaveFilesToDb(files, testPlan.Id, projectId);
    }

    public async Task<List<TestPlans>> GetTestPlansProjectTree(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestPlans
            .Where(p => p.ProjectsId == projectId)
            .Include(tp => tp.LinkedTestCases)
            .ToListAsync();
    }

    /// <summary>
    /// Update Card when drag and drop in db for TestPlan
    /// </summary>
    /// <param name="args"></param>
    public async Task UpdateCardOnDragDrop(RadzenDropZoneItemEventArgs<TestPlans> args)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.TestPlans.Update(args.Item);
        await db.SaveChangesAsync(userService);
    }

    public async Task<TestPlans> AddTestPlanFromCsv(TestPlans testPlan, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var existingCycles = await db.Cycles.Where(c => c.ProjectsId == projectId).ToListAsync();
        testPlan.ProjectsId = projectId;
        testPlan.WorkflowStatus = WorkflowStatus.New;
        testPlan.ArchivedStatus = ArchivedStatus.Active;

        var validCycle = testPlan.Cycle?.Name ?? string.Empty;

        testPlan.Cycle = existingCycles.FirstOrDefault(c => c.Name == validCycle);

        db.TestPlans.Add(testPlan);
        await db.SaveChangesAsync(userService);
        return testPlan;
    }

    public async Task DeleteTestPlan(TestPlans testPlans)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.TestPlans.Remove(testPlans);
        await db.SaveChangesAsync(userService);
    }
}