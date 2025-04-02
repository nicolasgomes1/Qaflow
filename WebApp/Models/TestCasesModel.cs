using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
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
    /// <summary>
    /// List of jira tickets to be displayed in the UI that  are fetched with the Jira API
    /// </summary>
    public List<JiraTask> JiraIntegrations = [];

    /// <summary>
    /// List of selected Jira tickets to be associated with the test case
    /// </summary>
    public List<string> SelectedJiraTicketIds { get; set; } = [];

    public List<TestSteps> TestStepsList { get; set; } = [];

    /// <summary>
    /// List of identifiers for the requirements that have been selected.
    /// These IDs correspond to the linked requirements associated with a test case.
    /// </summary>
    public List<int> SelectedRequirementIds { get; set; } = [];

    public string EstimatedTimeInput = string.Empty;

    public async Task<List<TestCases>> DisplayTestCasesIndexPage(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testcases = await db.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .Include(r => r.LinkedRequirements)
            .ToListAsync();

        return testcases;
    }

    public async Task<List<TestCases>> GetTestCasesWithWorkflowStatus(int projectId,
        WorkflowStatus workflowStatus = WorkflowStatus.Completed)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        return await db.TestCases
            .Where(tc => tc.ProjectsId == projectId && tc.WorkflowStatus == workflowStatus)
            .ToListAsync();
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="testCaseId"></param>
    /// <returns>TestCase with TestSteps and Requirements</returns>
    /// <exception cref="Exception"></exception>
    public async Task<TestCases> GetTestCaseData(int testCaseId)
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
    /// 
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

        //First create the test case
        db.TestCases.Add(testcase);
        testcase.ProjectsId = projectId;

        testcase.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testcase.EstimatedTime = TimeSpan.FromMinutes(int.TryParse(EstimatedTimeInput, out var minutes) ? minutes : 0);

        //adjust the active or not based on the workfllow
        if (testcase.WorkflowStatus == WorkflowStatus.Completed) testcase.ArchivedStatus = ArchivedStatus.Archived;

        // Assign test steps to TestCases before saving
        testcase.TestSteps = TestStepsList;
        foreach (var step in TestStepsList) step.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;


        await AddRequirementsDropdown(testcase);

        await AddJiraTickets(testcase);

        await db.SaveChangesAsync();


        // Process files
        if (files != null && files.Count != 0) await testCasesFilesModel.SaveFilesToDb(files, testcase.Id, projectId);

        return testcase;
    }

    private async Task AddRequirementsDropdown(TestCases testcase)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        if (testcase.LinkedRequirements is null)
            testcase.LinkedRequirements = [];

        // Fetch and add requirements asynchronously
        foreach (var requirementId in SelectedRequirementIds)
        {
            var requirement = await db.Requirements.FindAsync(requirementId);
            if (requirement == null)
                throw new Exception($"Requirement with ID {requirementId} not found.");

            testcase.LinkedRequirements.Add(requirement);
        }
    }

    private async Task UpdateRequirementsDropdown(TestCases testcase)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        if (testcase.LinkedRequirements is null)
            testcase.LinkedRequirements = [];

        // Load all selected requirements
        var selectedRequirements = await db.Requirements
            .Where(r => SelectedRequirementIds.Contains(r.Id))
            .ToListAsync();

        if (selectedRequirements.Count != SelectedRequirementIds.Count)
            throw new Exception("One or more selected requirements were not found");

        // Remove unselected requirements
        var toRemove = testcase.LinkedRequirements
            .Where(r => !SelectedRequirementIds.Contains(r.Id))
            .ToList();

        foreach (var requirement in toRemove) testcase.LinkedRequirements.Remove(requirement);

        // Add missing ones
        foreach (var requirement in selectedRequirements)
            if (testcase.LinkedRequirements.All(r => r.Id != requirement.Id))
                testcase.LinkedRequirements.Add(requirement);
    }


    public async Task UpdateTestCase(TestCases testCases, List<IBrowserFile>? files, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        db.Update(testCases);


        //adjust the active or not based on the workfllow
        if (testCases.WorkflowStatus == WorkflowStatus.Completed) testCases.ArchivedStatus = ArchivedStatus.Archived;
        testCases.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testCases.ModifiedAt = DateTime.UtcNow;

        var existingTestCase = await db.TestCases
            .AsSplitQuery()
            .Include(tc => tc.LinkedRequirements)
            .Include(testCases => testCases.TestSteps)
            .FirstOrDefaultAsync(tc => tc.Id == testCases.Id);

        if (existingTestCase == null) throw new Exception("Test case not found.");

        // Update the navigation property directly
        await UpdateRequirementsDropdown(testCases);
        await UpdateJiraTickets(testCases);


        // Update the ModifiedBy property for each test step
        foreach (var step in existingTestCase.TestSteps)
            if (step.ModifiedBy == null)
                step.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
            else
                step.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        await db.SaveChangesAsync();


        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await testCasesFilesModel.SaveFilesToDb(files, testCases.Id, projectId);
    }


    public async Task<List<TestSteps>> GetTestStepsForTestCase(int testCaseId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        var testCase = await db.TestCases
            .Include(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tc => tc.Id == testCaseId);

        if (testCase == null) throw new Exception($"Test case with ID {testCaseId} was not found.");

        return testCase.TestSteps.OrderBy(s => s.Number).ToList();
    }


    /// <summary>
    /// TestCase Jira Tickets that accepts a TestCases object and stores the selected Jira tickets
    /// </summary>
    /// <param name="testcase"></param>
    private async Task AddJiraTickets(TestCases testcase)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

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

        await db.SaveChangesAsync();
    }


    /// <summary>
    /// Updates the Jira tickets associated with a TestCases object
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
}