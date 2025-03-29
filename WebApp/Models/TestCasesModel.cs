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
    private readonly ApplicationDbContext _dbContext = dbContextFactory.CreateDbContext();

    /// <summary>
    /// List of jira tickets to be displayed in the UI that  are fetched with the Jira API
    /// </summary>
    public List<JiraTask> JiraIntegrations = [];

    /// <summary>
    /// List of selected Jira tickets to be associated with the test case
    /// </summary>
    public List<string> SelectedJiraTicketIds { get; set; } = [];

    public List<TestSteps> TestStepsList { get; set; } = [];

    public List<int> SelectedRequirementIds { get; set; } = [];


    public async Task<(IEnumerable<TestCases> TestCases, IList<TestCases> SelectedTestCases)>
        DisplayTestCasesIndexPage1(int projectId)
    {
        var testcases = await _dbContext.TestCases
            .Include(r => r.LinkedRequirements)
            .Where(tc => tc.ProjectsId == projectId)
            .ToListAsync();

        var selectedTestCases = new List<TestCases>();
        var selection = testcases.FirstOrDefault();
        if (selection != null) selectedTestCases.Add(selection);

        return (testcases, selectedTestCases);
    }

    public async Task<List<TestCases>> GetTestCasesWithWorkflowStatus(int projectId,
        WorkflowStatus workflowStatus = WorkflowStatus.Completed)
    {
        return await _dbContext.TestCases
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
        var testCase = await _dbContext.TestCases
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
        var requirement = await _dbContext.TestCases
            .Include(tc => tc.LinkedRequirements)
            .FirstOrDefaultAsync(tc => tc.Id == testCase.Id);

        if (requirement is { LinkedRequirements: not null })
            SelectedRequirementIds = requirement.LinkedRequirements.Select(r => r.Id).ToList();
        else
            throw new Exception("Error.");
    }


    public async Task<TestCases> AddTestCases(TestCases testcase, List<IBrowserFile>? files, int projectId)
    {
        //First create the test case
        _dbContext.TestCases.Add(testcase);
        testcase.ProjectsId = projectId;

        testcase.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        //adjust the active or not based on the workfllow
        if (testcase.WorkflowStatus == WorkflowStatus.Completed) testcase.ArchivedStatus = ArchivedStatus.Archived;

        // Assign test steps to TestCases before saving
        testcase.TestSteps = TestStepsList;
        foreach (var step in TestStepsList) step.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;


        await AddRequirementsDropdown(testcase);

        await _dbContext.SaveChangesAsync();


        await StoreJiraTickets(testcase);

        // Process files
        if (files != null && files.Count != 0) await testCasesFilesModel.SaveFilesToDb(files, testcase.Id, projectId);

        return testcase;
    }

    private async Task AddRequirementsDropdown(TestCases testcase)
    {
        // Initialize the collection if null
        testcase.LinkedRequirements = new List<Requirements>();

        // Load all selected requirements in one query
        var selectedRequirements = await _dbContext.Requirements
            .Where(r => SelectedRequirementIds.Contains(r.Id))
            .ToListAsync();

        if (selectedRequirements.Count != SelectedRequirementIds.Count)
            throw new Exception("One or more selected requirements were not found");

        // Add all requirements at once
        foreach (var requirement in selectedRequirements) testcase.LinkedRequirements.Add(requirement);
    }


    public async Task UpdateTestCase(TestCases testCases, List<IBrowserFile>? files, int projectId)
    {
        _dbContext.Update(testCases);


        //adjust the active or not based on the workfllow
        if (testCases.WorkflowStatus == WorkflowStatus.Completed) testCases.ArchivedStatus = ArchivedStatus.Archived;
        testCases.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testCases.ModifiedAt = DateTime.UtcNow;

        var existingTestCase = await _dbContext.TestCases
            .AsSplitQuery()
            .Include(tc => tc.LinkedRequirements)
            .Include(testCases => testCases.TestSteps)
            .FirstOrDefaultAsync(tc => tc.Id == testCases.Id);

        if (existingTestCase == null) throw new Exception("Test case not found.");

        // Update the navigation property directly
        existingTestCase.LinkedRequirements = await _dbContext.Requirements
            .Where(r => SelectedRequirementIds.Contains(r.Id))
            .ToListAsync();


        // Update the ModifiedBy property for each test step
        foreach (var step in existingTestCase.TestSteps)
            if (step.ModifiedBy == null)
                step.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
            else
                step.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;

        await _dbContext.SaveChangesAsync();

        await UpdateJiraTickets(testCases);

        // If there are files, attempt to save them
        if (files != null && files.Count != 0) await testCasesFilesModel.SaveFilesToDb(files, testCases.Id, projectId);
    }


    public async Task<List<TestSteps>> GetTestStepsForTestCase(int testCaseId)
    {
        var testCase = await _dbContext.TestCases
            .Include(tc => tc.TestSteps)
            .FirstOrDefaultAsync(tc => tc.Id == testCaseId);

        if (testCase == null) throw new Exception($"Test case with ID {testCaseId} was not found.");

        return testCase.TestSteps.OrderBy(s => s.Number).ToList();
    }


    /// <summary>
    /// TestCase Jira Tickets that accepts a TestCases object and stores the selected Jira tickets
    /// </summary>
    /// <param name="testcase"></param>
    private async Task StoreJiraTickets(TestCases testcase)
    {
        var selectedJiraTickets = JiraIntegrations
            .Where(jira => SelectedJiraTicketIds.Contains(jira.Id.ToString()))
            .Select(jira => jira.Key)
            .ToList();

        foreach (var testCaseJira in selectedJiraTickets.Select(jiraKey => new TestCasesJira
                 {
                     TestCasesJiraId = testcase.Id,
                     Key = jiraKey,
                     JiraId = Convert.ToInt32(JiraIntegrations.First(jira => jira.Key == jiraKey).Id.ToString())
                 }))
            _dbContext.TestCasesJira.Add(testCaseJira);

        await _dbContext.SaveChangesAsync();
    }


    /// <summary>
    /// Updates the Jira tickets associated with a TestCases object
    /// </summary>
    /// <param name="testcase"></param>
    private async Task UpdateJiraTickets(TestCases testcase)
    {
        // Retrieve existing Jira ticket associations for the given TestCase
        var existingJiraTickets = await _dbContext.TestCasesJira
            .Where(tcj => tcj.TestCasesJiraId == testcase.Id)
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
                TestCasesJiraId = testcase.Id,
                Key = jira.Key,
                JiraId = jira.Value
            });

        // Determine which Jira tickets need to be removed
        var ticketsToRemove = existingJiraTickets
            .Where(tcj => !selectedJiraKeys.ContainsKey(tcj.Key));

        // Add new Jira tickets to the context
        _dbContext.TestCasesJira.AddRange(ticketsToAdd);

        // Remove obsolete Jira tickets from the context
        _dbContext.TestCasesJira.RemoveRange(ticketsToRemove);

        // Save changes to the database
        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<TestCases>> GetTestCasesToValidateAgainstCsv(int projectId)
    {
        return await _dbContext.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .Include(tc => tc.TestSteps)
            .ToListAsync();
    }

    public async Task AddTestCasesFromCsv(TestCases testCase, int projectId)
    {
        _dbContext.TestCases.Add(testCase);
        testCase.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        testCase.ProjectsId = projectId;
        testCase.WorkflowStatus = WorkflowStatus.New;
        testCase.CreatedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();
    }


    public async Task<List<TestCases>> GetTestCasesForProjectTree(int projectId)
    {
        return await _dbContext.TestCases
            .Where(p => p.ProjectsId == projectId)
            .Include(tc => tc.TestPlans)
            .ToListAsync();
    }
}