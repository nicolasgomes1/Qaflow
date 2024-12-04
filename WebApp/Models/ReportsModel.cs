using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class ReportsModel
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ApplicationDbContext _dbContext;
    private readonly ProjectStateService _projectStateService;

    public ReportsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory,
        ProjectStateService projectStateService)
    {
        _dbContextFactory = dbContextFactory;
        _dbContext = _dbContextFactory.CreateDbContext();
        _projectStateService = projectStateService;
    }

    public async Task<(double, double)> GetTestCasePercentagesAsync()
    {
        await using var db1 = await _dbContextFactory.CreateDbContextAsync();
        await using var db2 = await _dbContextFactory.CreateDbContextAsync();


        // Count the total number of TestCases
        var totalTestCases = await db1.TestCases
            .Where(tc => tc.ProjectsId == _projectStateService.ProjectId)
            .CountAsync();

        // Count the number of TestCases that have at least one Requirement
        var testCasesWithRequirements = await db2.TestCases
            .Where(tc => tc.ProjectsId == _projectStateService.ProjectId)
            .Where(tc => tc.Requirements.Count != 0)
            .CountAsync();

        // Calculate the number of TestCases without Requirements
        var testCasesWithoutRequirements = totalTestCases - testCasesWithRequirements;

        // Calculate percentages (default to 0 if no total test cases)
        var testCasesWithRequirementsPercentage = totalTestCases > 0
            ? (double)testCasesWithRequirements / totalTestCases * 100
            : 0;

        var testCasesWithoutRequirementsPercentage = totalTestCases > 0
            ? (double)testCasesWithoutRequirements / totalTestCases * 100
            : 0;

        return (Math.Round(testCasesWithRequirementsPercentage, 2),
            Math.Round(testCasesWithoutRequirementsPercentage, 2));
    }

    public async Task<(double, double)> GetTestPlansPercentagesAsync()
    {
        await using var db1 = await _dbContextFactory.CreateDbContextAsync();
        await using var db2 = await _dbContextFactory.CreateDbContextAsync();

        var testPlansWithTestCases = await db1.TestPlans
            .Where(tc => tc.ProjectsId == _projectStateService.ProjectId)
            .Where(tc => tc.TestCases.Any())
            .CountAsync();

        var testPlansWithoutTestCases = await db2.TestPlans
            .Where(tc => tc.ProjectsId == _projectStateService.ProjectId)
            .Where(tc => !tc.TestCases.Any())
            .CountAsync();

        var totalTestPlans = testPlansWithTestCases + testPlansWithoutTestCases;

        float testPlansWithTestCasesPercentage = 0;
        float testPlansWithoutTestCasesPercentage = 0;

        if (totalTestPlans <= 0)
            return (Math.Round(testPlansWithTestCasesPercentage, 2),
                Math.Round(testPlansWithoutTestCasesPercentage, 2));
        testPlansWithTestCasesPercentage = (float)testPlansWithTestCases / totalTestPlans * 100;
        testPlansWithoutTestCasesPercentage = (float)testPlansWithoutTestCases / totalTestPlans * 100;

        return (Math.Round(testPlansWithTestCasesPercentage, 2), Math.Round(testPlansWithoutTestCasesPercentage, 2));
    }

    public async Task<(double, double, double)> GetTestExecutionsPercentagesAsync()
    {
        await using var db1 = await _dbContextFactory.CreateDbContextAsync();
        await using var db2 = await _dbContextFactory.CreateDbContextAsync();
        await using var db3 = await _dbContextFactory.CreateDbContextAsync();

        var testExecutionsPassed = await db1.TestExecution
            .Where(te => te.TEProjectId == _projectStateService.ProjectId)
            .Where(te => te.ExecutionStatus == ExecutionStatus.Passed)
            .CountAsync();

        var testExecutionsFailed = await db2.TestExecution
            .Where(te => te.TEProjectId == _projectStateService.ProjectId)
            .Where(te => te.ExecutionStatus == ExecutionStatus.Failed)
            .CountAsync();

        var testExecutionsNotRun = await db3.TestExecution
            .Where(te => te.TEProjectId == _projectStateService.ProjectId)
            .Where(te => te.ExecutionStatus == ExecutionStatus.NotRun)
            .CountAsync();

        var totalTestExecutions = testExecutionsPassed + testExecutionsFailed + testExecutionsNotRun;

        var testExecutionsPassedPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsPassed / totalTestExecutions * 100 : 0;
        var testExecutionsFailedPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsFailed / totalTestExecutions * 100 : 0;
        var testExecutionNotRunPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsNotRun / totalTestExecutions * 100 : 0;
        return (Math.Round(testExecutionsPassedPercentage, 2), Math.Round(testExecutionsFailedPercentage, 2),
            Math.Round(testExecutionNotRunPercentage, 2));
    }

    public string LoadTotalRequirements()
    {
        var projectRequirements = _dbContext.Requirements.Count(r => r.ProjectsId == _projectStateService.ProjectId)
            .ToString();

        return projectRequirements == "0" ? "No Requirements" : $"Requirements: {projectRequirements}";
    }

    public string LoadTotalTestCases()
    {
        var projectTestCases = _dbContext.TestCases.Count(tc => tc.ProjectsId == _projectStateService.ProjectId)
            .ToString();

        return projectTestCases == "0" ? "No Test Cases" : $"Test Cases: {projectTestCases}";
    }

    public string LoadTotalTestPlans()
    {
        var projectTestPlans = _dbContext.TestPlans.Count(tp => tp.ProjectsId == _projectStateService.ProjectId)
            .ToString();

        return projectTestPlans == "0" ? "No Test Plans" : $"Test Plans: {projectTestPlans}";
    }

    public string LoadTotalTestExecutions()
    {
        var projectTestExecutions = _dbContext.TestExecution
            .Count(te => te.TEProjectId == _projectStateService.ProjectId).ToString();

        return projectTestExecutions == "0" ? "No Test Executions" : $"Test Executions: {projectTestExecutions}";
    }

    public string LoadTotalBugs()
    {
        var projectBugs = _dbContext.Bugs.Count(b => b.ProjectsId == _projectStateService.ProjectId).ToString();
        return projectBugs == "0" ? "No Bugs" : $"Bugs: {projectBugs}";
    }
}