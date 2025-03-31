using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class ReportsModel
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ApplicationDbContext _dbContext;

    public ReportsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _dbContext = _dbContextFactory.CreateDbContext();
    }

    public async Task<(double, double)> GetTestCasePercentagesAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var totalTestCases = await db.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .CountAsync();

        var testCasesWithRequirements = await db.TestCases
            .Where(tc => tc.ProjectsId == projectId)
            .CountAsync(tc => tc.LinkedRequirements.Any());

        var testCasesWithoutRequirements = totalTestCases - testCasesWithRequirements;

        var testCasesWithRequirementsPercentage = totalTestCases > 0
            ? (double)testCasesWithRequirements / totalTestCases * 100
            : 0;

        var testCasesWithoutRequirementsPercentage = totalTestCases > 0
            ? (double)testCasesWithoutRequirements / totalTestCases * 100
            : 0;

        return (Math.Round(testCasesWithRequirementsPercentage, 2),
            Math.Round(testCasesWithoutRequirementsPercentage, 2));
    }

    public async Task<(double, double)> GetTestPlansPercentagesAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var testPlans = await db.TestPlans
            .Where(tp => tp.ProjectsId == projectId)
            .Select(tp => new { tp.Id, HasTestCases = tp.LinkedTestCases.Any() })
            .ToListAsync();

        var testPlansWithTestCases = testPlans.Count(tp => tp.HasTestCases);
        var testPlansWithoutTestCases = testPlans.Count(tp => !tp.HasTestCases);
        var totalTestPlans = testPlans.Count;

        double testPlansWithTestCasesPercentage = 0;
        double testPlansWithoutTestCasesPercentage = 0;

        if (totalTestPlans > 0)
        {
            testPlansWithTestCasesPercentage = (double)testPlansWithTestCases / totalTestPlans * 100;
            testPlansWithoutTestCasesPercentage = (double)testPlansWithoutTestCases / totalTestPlans * 100;
        }

        return (Math.Round(testPlansWithTestCasesPercentage, 2),
            Math.Round(testPlansWithoutTestCasesPercentage, 2));
    }

    public async Task<(double, double, double)> GetTestExecutionsPercentagesAsync(int projectId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var executionCounts = await db.TestExecution
            .Where(te => te.ProjectsId == projectId)
            .GroupBy(te => te.ExecutionStatus)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);

        var testExecutionsPassed = executionCounts.GetValueOrDefault(ExecutionStatus.Passed);
        var testExecutionsFailed = executionCounts.GetValueOrDefault(ExecutionStatus.Failed);
        var testExecutionsNotRun = executionCounts.GetValueOrDefault(ExecutionStatus.NotRun);

        var totalTestExecutions = testExecutionsPassed + testExecutionsFailed + testExecutionsNotRun;

        var testExecutionsPassedPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsPassed / totalTestExecutions * 100 : 0;
        var testExecutionsFailedPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsFailed / totalTestExecutions * 100 : 0;
        var testExecutionNotRunPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsNotRun / totalTestExecutions * 100 : 0;

        return (Math.Round(testExecutionsPassedPercentage, 2),
            Math.Round(testExecutionsFailedPercentage, 2),
            Math.Round(testExecutionNotRunPercentage, 2));
    }

    public async Task<string> LoadTotalRequirements(int projectId)
    {
        var projectRequirementsCount = await TotalRequirements(projectId);
        var projectReq = projectRequirementsCount.ToString();

        return projectReq == "0" ? "No Requirements" : $"Requirements: {projectReq}";
    }

    private async Task<int> TotalRequirements(int projectId)
    {
        var projectRequirements = await _dbContext.Requirements.CountAsync(r => r.ProjectsId == projectId);
        return projectRequirements;
    }

    public string LoadTotalTestCases(int projectId)
    {
        var projectTestCases = _dbContext.TestCases.Count(tc => tc.ProjectsId == projectId)
            .ToString();

        return projectTestCases == "0" ? "No Test Cases" : $"Test Cases: {projectTestCases}";
    }

    public string LoadTotalTestPlans(int projectId)
    {
        var projectTestPlans = _dbContext.TestPlans.Count(tp => tp.ProjectsId == projectId)
            .ToString();

        return projectTestPlans == "0" ? "No Test Plans" : $"Test Plans: {projectTestPlans}";
    }

    public string LoadTotalTestExecutions(int projectId)
    {
        var projectTestExecutions = _dbContext.TestExecution
            .Count(te => te.ProjectsId == projectId).ToString();

        return projectTestExecutions == "0" ? "No Test Executions" : $"Test Executions: {projectTestExecutions}";
    }

    public string LoadTotalBugs(int projectId)
    {
        var projectBugs = _dbContext.Bugs.Count(b => b.ProjectsId == projectId).ToString();
        return projectBugs == "0" ? "No Bugs" : $"Bugs: {projectBugs}";
    }


    private async Task<int> TotalRequirementsSpecifications(int projectId)
    {
        var projectRequirementsSpecifications = await _dbContext.Requirements
            .Where(r => r.ProjectsId == projectId)
            .CountAsync(r => r.RequirementsSpecification != null && r.RequirementsSpecification.LinkedRequirements.Any());
        return projectRequirementsSpecifications;
    }

    public async Task<double> RequirementsCoveredBySpecifications(int projectId)
    {
        var projectRequirementsSpecifications = await TotalRequirementsSpecifications(projectId);
        var projectRequirements = await TotalRequirements(projectId);
        var projectRequirementsCoveredBySpecifications = projectRequirementsSpecifications == 0
            ? 0
            : (double)projectRequirementsSpecifications / projectRequirements * 100;
        return Math.Round(projectRequirementsCoveredBySpecifications, 2);
    }

    public async Task<string> RequirementsWithRequirementspecifications(int projectId)
    {
        // Count the Requirements which are linked to at least one Requirementspecification in the given project.
        var requirementsWithSpecificationsCount = await TotalRequirementsSpecifications(projectId);

        // Return a message based on the count of Requirements linked to RequirementSpecifications.
        return requirementsWithSpecificationsCount == 0
            ? "No Requirements with Spec"
            : $"Requirements with Spec: {requirementsWithSpecificationsCount}";
    }


    public async Task<double> GetTestExecutionPassRateAsync(int projectId)
    {
        await using var db1 = await _dbContextFactory.CreateDbContextAsync();
        await using var db2 = await _dbContextFactory.CreateDbContextAsync();

        var testExecutionsPassed = await db1.TestExecution
            .Where(te => te.ProjectsId == projectId)
            .Where(te => te.ExecutionStatus == ExecutionStatus.Passed)
            .Where(te => te.IsActive == false)
            .CountAsync();

        var testExecutionsFailed = await db2.TestExecution
            .Where(te => te.ProjectsId == projectId)
            .Where(te => te.ExecutionStatus == ExecutionStatus.Failed)
            .Where(te => te.IsActive == false)
            .CountAsync();

        var totalTestExecutions = testExecutionsPassed + testExecutionsFailed;

        var testExecutionsPassedPercentage =
            totalTestExecutions > 0 ? (double)testExecutionsPassed / totalTestExecutions * 100 : 0;

        return Math.Round(testExecutionsPassedPercentage, 2);
    }
}