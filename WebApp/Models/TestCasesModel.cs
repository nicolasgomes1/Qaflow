using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class TestCasesModel(
    IDbContextFactory<ApplicationDbContext> dbContextFactory,
    UserService userService,
    TestCasesFilesModel testCasesFilesModel)
{
    public string EstimatedTimeInput = string.Empty;

    /// <summary>
    ///     List of jira tickets to be displayed in the UI that  are fetched with the Jira API
    /// </summary>
    public List<JiraTask> JiraIntegrations = [];

    /// <summary>
    ///     List of selected Jira tickets to be associated with the test case
    /// </summary>
    public List<string> SelectedJiraTicketIds { get; set; } = [];

    public List<TestSteps> TestStepsList { get; set; } = [];

    /// <summary>
    ///     List of identifiers for the requirements that have been selected.
    ///     These IDs correspond to the linked requirements associated with a test case.
    /// </summary>
    public List<int> SelectedRequirementIds { get; set; } = [];

    public async Task<List<TestCases>> DisplayTestCasesIndexPage(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testcases = await db.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .Include(r => r.LinkedRequirements)
            .ToListAsync();

        return testcases;
    }

    /// <summary>
    ///     Value to be used in dropdowns
    /// </summary>
    /// <param name="projectId"></param>
    /// <param name="workflowStatus"></param>
    /// <returns></returns>
    public async Task<List<TestCases>> GetTestCasesWithWorkflowStatus(int projectId,
        WorkflowStatus workflowStatus = WorkflowStatus.Completed)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCases
            .Where(tc => tc.ProjectsId == projectId && tc.WorkflowStatus == workflowStatus)
            .ToListAsync();
    }


    /// <summary>
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns>TestCase with TestSteps and Requirements</returns>
    /// <exception cref="Exception"></exception>
    public async Task<TestCases> GetTestCasesByIdAsync(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testCase = await db.TestCases
            .AsSplitQuery()
            .Include(tc => tc.TestSteps)
            .Include(tc => tc.LinkedRequirements)
            .FirstOrDefaultAsync(tc => tc.Id == testCaseId);

        return testCase ?? throw new Exception("Test case not found.");
    }

    /// <summary>
    /// </summary>
    /// <returns>A list of associated requirements witht the TestCase</returns>
    /// <exception cref="Exception"></exception>
    public async Task GetAssociatedRequirements(TestCases testCase)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var requirement = await db.TestCases
            .Include(tc => tc.LinkedRequirements)
            .FirstOrDefaultAsync(tc => tc.Id == testCase.Id);

        if (requirement is { LinkedRequirements: not null })
            SelectedRequirementIds = requirement.LinkedRequirements.Select(r => r.Id).ToList();
        else
            throw new Exception("Error.");
    }


    public async Task<TestCases> AddTestCases(TestCases testcase, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.TestCases.Add(testcase);

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(testcase);

        testcase.ProjectsId = projectId;
        testcase.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testcase.EstimatedTime = TimeSpan.FromMinutes(int.TryParse(EstimatedTimeInput, out var minutes) ? minutes : 0);

        if (testcase.WorkflowStatus == WorkflowStatus.Completed) testcase.ArchivedStatus = ArchivedStatus.Archived;

        // Assign test steps to TestCases before saving
        testcase.TestSteps = TestStepsList;
        foreach (var step in TestStepsList) step.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;


        await AddRequirementsDropdown(testcase, db);

        await AddJiraTickets(testcase, db);

        await db.SaveChangesAsync();


        // Process files
        if (files != null && files.Count != 0) await testCasesFilesModel.SaveFilesToDb(files, testcase.Id, projectId);

        return testcase;
    }

    private async Task AddRequirementsDropdown(TestCases testcase, ApplicationDbContext dbContext)
    {
        // Initialize the collection
        testcase.LinkedRequirements = [];

        // Fetch all requirements in a single query
        var requirements = await dbContext.Requirements
            .Where(r => SelectedRequirementIds.Contains(r.Id))
            .ToListAsync();

        // Verify all requirements were found
        if (requirements.Count != SelectedRequirementIds.Count)
            throw new Exception("One or more selected requirements were not found.");

        // Assign all requirements at once
        testcase.LinkedRequirements = requirements;
    }

    private async Task UpdateRequirementsDropdown(TestCases testcase, ApplicationDbContext dbContext)
    {
        var existingTestCase = await dbContext.TestCases
            .Include(tc => tc.LinkedRequirements) // Ensures navigation property is loaded
            .FirstOrDefaultAsync(tc => tc.Id == testcase.Id);

        if (existingTestCase is null)
            throw new Exception($"TestCase with ID {testcase.Id} not found.");

        var selectedRequirements = await dbContext.Requirements
            .Where(r => SelectedRequirementIds.Contains(r.Id))
            .ToListAsync();


        if (selectedRequirements.Count != SelectedRequirementIds.Count)
            throw new Exception("One or more selected requirements were not found.");

        existingTestCase.LinkedRequirements = selectedRequirements;

        await dbContext.SaveChangesAsync();
    }


    public async Task UpdateTestCase(TestCases testCases, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.Entry(testCases).State = EntityState.Modified;


        await UpdateRequirementsDropdown(testCases, db);
        await UpdateJiraTickets(testCases);

        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(testCases);
        testCases.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testCases.ModifiedAt = DateTime.UtcNow;
        testCases.TestSteps = TestStepsList;

        await db.SaveChangesAsync();

        if (files is { Count: > 0 })
            await testCasesFilesModel.SaveFilesToDb(files, testCases.Id, projectId);
    }


    /// <summary>
    ///     only return active test cases
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<List<TestSteps>> GetTestStepsForTestCase(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testCase = await db.TestCases
            .Include(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tc => tc.Id == testCaseId);

        if (testCase == null) throw new Exception($"Test case with ID {testCaseId} was not found.");

        return testCase.TestSteps.OrderBy(s => s.Number).ToList();
    }

    public async Task<List<TestSteps>> GetTestStepsForTestCaseAsync(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestSteps.Where(s => s.TestCasesId == testCaseId && s.ArchivedStatus == ArchivedStatus.Active)
            .ToListAsync();
    }


    /// <summary>
    ///     TestCase Jira Tickets that accepts a TestCases object and stores the selected Jira tickets
    /// </summary>
    /// <param name="testcase"></param>
    /// <param name="db"></param>
    private async Task AddJiraTickets(TestCases testcase, ApplicationDbContext db)
    {
        var selectedJiraTickets = JiraIntegrations
            .Where(jira => SelectedJiraTicketIds.Contains(jira.Id.ToString()))
            .Select(jira => jira.Key)
            .ToList();

        foreach (var testCaseJira in selectedJiraTickets
                     .Select(jiraKey => new TestCasesJira
                     {
                         TestCases = testcase,
                         Key = jiraKey,
                         JiraId = Convert.ToInt32(JiraIntegrations.First(jira => jira.Key == jiraKey).Id.ToString())
                     }))
            db.TestCasesJira.Add(testCaseJira);
        await Task.CompletedTask;
        //   await db.SaveChangesAsync();
    }


    /// <summary>
    ///     Updates the Jira tickets associated with a TestCases object
    /// </summary>
    /// <param name="testcase"></param>
    private async Task UpdateJiraTickets(TestCases testcase)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        // Retrieve existing Jira ticket associations for the given TestCase
        var existingJiraTickets = await db.TestCasesJira
            .Where(tcj => tcj.TestCases == testcase)
            .ToListAsync();

        // Get the selected Jira ticket keys to associate with the TestCase
        var selectedJiraKeys = JiraIntegrations
            .Where(jira => SelectedJiraTicketIds.Contains(jira.Id.ToString()))
            .ToDictionary(jira => jira.Key, jira => Convert.ToInt32(jira.Id.ToString()));

        // Extract existing Jira keys from database records
        var existingJiraKeys = existingJiraTickets
            .ToDictionary(tcj => tcj.Key, tcj => tcj);

        // Determine which Jira tickets need to be added
        var ticketsToAdd = selectedJiraKeys
            .Where(jira => !existingJiraKeys.ContainsKey(jira.Key))
            .Select(jira => new TestCasesJira
            {
                TestCases = testcase,
                Key = jira.Key,
                JiraId = jira.Value
            });

        // Determine which Jira tickets need to be removed
        var ticketsToRemove = existingJiraTickets
            .Where(tcj => !selectedJiraKeys.ContainsKey(tcj.Key));

        // Add new Jira tickets to the context
        db.TestCasesJira.AddRange(ticketsToAdd);

        // Remove obsolete Jira tickets from the context
        db.TestCasesJira.RemoveRange(ticketsToRemove);

        // Save changes to the database
        await db.SaveChangesAsync();
    }

    public async Task<List<TestCases>> GetTestCasesToValidateAgainstCsv(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .Include(tc => tc.TestSteps)
            .ToListAsync();
    }

    public async Task AddTestCasesFromCsv(TestCases testCase, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.TestCases.Add(testCase);
        testCase.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testCase.ProjectsId = projectId;
        testCase.WorkflowStatus = WorkflowStatus.New;
        testCase.CreatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
    }


    public async Task<List<TestCases>> GetTestCasesForProjectTree(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCases
            .Where(p => p.ProjectsId == projectId)
            .Include(tc => tc.TestPlans)
            .ToListAsync();
    }

    public async Task DeleteTestCase(TestCases testCases)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.TestCases.Remove(testCases);
        await db.SaveChangesAsync();
    }

    public async Task<List<TestCases>> GetTestCasesAssignedToAll(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestCases.Where(rp => rp.ProjectsId == projectId).ToListAsync();
    }

    public async Task<List<TestCases>> GetTestCasesAssignedToCurrentUser(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.TestCases.Where(rp =>
                rp.ProjectsId == projectId && rp.AssignedTo == userService.GetCurrentUserInfoAsync().Result.UserId)
            .ToListAsync();
    }

    /// <summary>
    ///     Update Card when drag and drop in db for TestCases
    /// </summary>
    /// <param name="args"></param>
    public async Task UpdateCardOnDragDrop(RadzenDropZoneItemEventArgs<TestCases> args)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        args.Item.ModifiedBy = userService.GetCurrentUserNameAsync().Result;
        args.Item.ModifiedAt = DateTime.UtcNow;
        db.TestCases.Update(args.Item);
        await db.SaveChangesAsync();
    }
}