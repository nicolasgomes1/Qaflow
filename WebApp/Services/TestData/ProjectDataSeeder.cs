using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Models;

namespace WebApp.Services.TestData;

public class ProjectDataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    private const string USER = "user@example.com";
    private const string MANAGER = "manager@example.com";
    private const string ADMIN = "admin@example.com";

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("SeedingLogger");

        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var
            dbContext =
                await dbContextFactory.CreateDbContextAsync(cancellationToken); // Use using for context disposal
        logger.LogInformation("Starting ProjectDataSeeder");
        // Seeding process
        await SeedDataAsync(dbContext);
        logger.LogInformation("Finished Seeding ProjectDataSeeder");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // Nothing to clean up for this case
    }


    private static async Task SeedDataAsync(ApplicationDbContext dbContext)
    {
        /// create list of test steps
        var testSteps = new List<TestSteps>
        {
            new()
            {
                Number = 1,
                Description = "Step 1 Description",
                ExpectedResult = "Step 1 Expected Result"
            }
        };


        // Create or get project
        var project = await GetOrCreateProjectAsync(dbContext, "Demo Project With Data");
        await GetOrCreateProjectAsync(dbContext, "Demo Project Without Data");


        //Create or get Requirements Specification
        var requirementsSpecification1 =
            await GetOrCreateRequirementsSpecificationAsync(dbContext, project, "Requirements Specification 1");
        var requirementsSpecification2 =
            await GetOrCreateRequirementsSpecificationAsync(dbContext, project, "Requirements Specification 2");
        var requirementsSpecification3 =
            await GetOrCreateRequirementsSpecificationAsync(dbContext, project, "Requirements Specification 3");

        // Create or get requirement
        var requirement =
            await GetOrCreateRequirementAsync(dbContext, project, "Requirement A", "requirement is A", USER,
                requirementsSpecification1);

        await GetOrCreateRequirementAsync(dbContext, project, "Requirement B", "requirement is B", MANAGER);

        for (var i = 1; i <= 20; i++)
            await GetOrCreateRequirementAsync(dbContext, project, $"Requirement {i}", $"requirement number is {i}",
                MANAGER);

        for (var i = 1; i <= 5; i++)
            await GetOrCreateRequirementAsync(dbContext, project, $"Requirement Cmpleted {i}",
                $"requirement number is {i}",
                MANAGER, null, WorkflowStatus.Completed);

        // Create or get test cases
        var testCase1 =
            await GetOrCreateTestCaseAsync(dbContext, project, "Test Case 1", "Sample test case 1", USER, null);
        var testCase2 =
            await GetOrCreateTestCaseAsync(dbContext, project, "Test Case 2", "Sample test case 2", MANAGER, null);
        await GetOrCreateTestCaseAsync(dbContext, project, "Test Case 3", "Sample test case ", USER, requirement);

        for (var i = 1; i <= 10; i++)
            await GetOrCreateTestCaseAsync(dbContext, project, $"Test Case {i}.", $"Test Case number is {i}.", USER,
                null);

        await GetOrCreateTestCaseWithStepsAsync(dbContext, project, "Test Case With Test Steps", "Sample test case 4",
            null,
            testSteps);


        var cycle = await GetOrCreateCycle(dbContext, project, "Cycle 1");
        await GetOrCreateCycle(dbContext, project, "Cycle 2");
        await GetOrCreateCycle(dbContext, project, "Cycle 3");
        await GetOrCreateCycle(dbContext, project, "Cycle 4");
        await GetOrCreateCycle(dbContext, project, "Cycle 5");

        // Create or get test plans associated with the test cases
        var testplan = await GetOrCreateTestPlanAsync(dbContext, project, "Test Plan Alpha", "Alpha", USER, testCase1,
            cycle,
            WorkflowStatus.Completed);
        await GetOrCreateTestPlanAsync(dbContext, project, "Test Plan Alpha", "Beta", MANAGER, testCase2, cycle,
            WorkflowStatus.InReview);
        await GetOrCreateTestPlanAsync(dbContext, project, "Test Plan Beta", "no tests", USER, null, cycle,
            WorkflowStatus.New);

        await GetOrCreateTestExecutionAsync(dbContext, project, testplan.Name, "Test Execution 1");

        await GetOrCreateBugsAsync(dbContext, project, "Bug 1", "Bug 1 Description", USER);
        await GetOrCreateBugsAsync(dbContext, project, "Bug 2", "Bug 2 Description", USER, BugStatus.Closed);
        await GetOrCreateBugsAsync(dbContext, project, "Bug 3", "Bug 3 Description", USER, BugStatus.InProgress);
        await GetOrCreateBugsAsync(dbContext, project, "Bug 4", "Bug 4 Description", MANAGER, BugStatus.InReview);
        await GetOrCreateBugsAsync(dbContext, project, "Bug 5", "Bug 5 Description", MANAGER, BugStatus.Open);
    }

    public static async Task<Projects> GetOrCreateProjectAsync(ApplicationDbContext dbContext, string projectName)
    {
        var logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger("Projects");

        var project = await dbContext.Projects.FirstOrDefaultAsync(p => p.Name == projectName)
                      ??
                      new Projects
                      {
                          Name = projectName,
                          Description = "Project A Description",
                          CreatedBy = USER,
                          ArchivedStatus = ArchivedStatus.Active
                      };

        if (project.Id != 0)
        {
            logger.LogInformation("Project {ProjectName} already exists", projectName);
            return project;
        }

        await dbContext.Projects.AddAsync(project);
        await dbContext.SaveChangesAsync();
        logger.LogInformation("Project {ProjectName} created", projectName);
        return project;
    }


    private static async Task<Requirements> GetOrCreateRequirementAsync(ApplicationDbContext dbContext,
        Projects projects,
        string name, string description, string assignedUserName, RequirementsSpecification? reqSpec = null,
        WorkflowStatus status = WorkflowStatus.New)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        // Check if the requirement already exists
        var existingRequirement = await dbContext.Requirements
            .FirstOrDefaultAsync(r => r.Projects == projects && r.Name == name);

        if (existingRequirement != null) return existingRequirement; // Return existing requirement

        // Create new requirement if it doesn't exist
        var newRequirement = new Requirements
        {
            Name = name,
            Description = description,
            Projects = projects,
            AssignedTo = assignedUserId,
            CreatedBy = ADMIN,
            RequirementsSpecification = reqSpec,
            WorkflowStatus = status
        };

        await dbContext.Requirements.AddAsync(newRequirement);
        SetArchivedStatus.SetArchivedStatusBasedOnWorkflow(newRequirement);

        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newRequirement;
    }

    private static async Task<string> AssignedUserId(ApplicationDbContext dbContext, string assignedUserName)
    {
        // Retrieve the user Id for the assigned user
        var assignedUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == assignedUserName);

        if (assignedUser == null)
            throw new InvalidOperationException($"User with UserName '{assignedUserName}' not found.");

        var assignedUserId = assignedUser.Id;
        return assignedUserId;
    }


    private static async Task<RequirementsSpecification> GetOrCreateRequirementsSpecificationAsync(
        ApplicationDbContext dbContext, Projects projects, string name)
    {
        var currentSpecification = await dbContext.RequirementsSpecification
            .FirstOrDefaultAsync(rs => rs.Projects == projects && rs.Name == name);
        if (currentSpecification != null) return currentSpecification;

        var newSpecification = new RequirementsSpecification
        {
            Name = name,
            Description = "This is a description for the requirements specification",
            Projects = projects,
            CreatedBy = USER
        };
        await dbContext.RequirementsSpecification.AddAsync(newSpecification);
        await dbContext.SaveChangesAsync();
        return newSpecification;
    }


    private static async Task<Bugs> GetOrCreateBugsAsync(ApplicationDbContext dbContext, Projects projects, string name,
        string description, string assignedUserName, BugStatus status = BugStatus.Open)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);


        var existingBug = await dbContext.Bugs
            .FirstOrDefaultAsync(b => b.Projects == projects && b.Name == name);

        if (existingBug != null) return existingBug;

        var newBug = new Bugs
        {
            Name = name,
            Description = description,
            Projects = projects,
            CreatedBy = USER,
            BugStatus = status,
            AssignedTo = assignedUserId,
            WorkflowStatus = WorkflowStatus.New
        };

        await dbContext.Bugs.AddAsync(newBug);
        await dbContext.SaveChangesAsync();
        return newBug;
    }


    private static async Task<TestCases> GetOrCreateTestCaseAsync(ApplicationDbContext dbContext, Projects projects,
        string name, string description, string assignedUserName, Requirements? requirements)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        // Check if the test case already exists
        var existingTestCase = await dbContext.TestCases
            .Include(tc => tc.LinkedRequirements) // Include Requirements to avoid lazy loading
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.Projects == projects);

        if (existingTestCase != null) return existingTestCase; // Return existing test case

        // Create new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            Projects = projects,
            CreatedBy = USER,
            AssignedTo = assignedUserId,
            WorkflowStatus = WorkflowStatus.Completed
        };

        if (newTestCase.WorkflowStatus == WorkflowStatus.Completed)
            newTestCase.ArchivedStatus = ArchivedStatus.Archived;

        if (requirements != null)
        {
            if (newTestCase.LinkedRequirements == null)
                throw new InvalidOperationException("Requirements collection is not initialized");

            newTestCase.LinkedRequirements.Add(requirements); // Add the requirement if it's provided
        }

        await dbContext.TestCases.AddAsync(newTestCase);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newTestCase;
    }

    private static async Task<TestCases> GetOrCreateTestCaseWithStepsAsync(
        ApplicationDbContext dbContext,
        Projects projects,
        string name,
        string description,
        Requirements? requirements,
        List<TestSteps>? testStepsList // Accept a list of test steps
    )
    {
        // Check if the test case already exists
        var existingTestCase = await dbContext.TestCases
            .AsSplitQuery()
            .Include(tc => tc.LinkedRequirements) // Include Requirements to avoid lazy loading
            .Include(tc => tc.TestSteps) // Include TestSteps to avoid lazy loading
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.Projects == projects);

        if (existingTestCase != null) return existingTestCase; // Return existing test case

        // Create a new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            Projects = projects,
            CreatedBy = USER,
            WorkflowStatus = WorkflowStatus.Completed
        };

        // Add the requirement if it's provided
        if (requirements != null)
        {
            if (newTestCase.LinkedRequirements == null)
                throw new InvalidOperationException("Requirements collection is not initialized");

            newTestCase.LinkedRequirements.Add(requirements);
        }

        // Add test steps if the list is not null and has elements
        if (testStepsList != null && testStepsList.Any())
        {
            if (newTestCase.TestSteps == null)
                throw new InvalidOperationException("TestSteps collection is not initialized");

            foreach (var step in testStepsList)
            {
                step.TestCasesId = newTestCase.Id; // Link step to this test case by setting the foreign key
                newTestCase.TestSteps.Add(step); // Add the step to the test case's TestSteps collection
            }
        }

        await dbContext.TestCases.AddAsync(newTestCase);
        await dbContext.SaveChangesAsync(); // Save to get the Id and link TestSteps

        return newTestCase;
    }


    private static async Task<TestPlans> GetOrCreateTestPlanAsync(
        ApplicationDbContext dbContext,
        Projects projects,
        string name,
        string description,
        string assignedUserName,
        TestCases? testCase,
        Cycles? cycle,
        WorkflowStatus status = WorkflowStatus.Completed)
    {
        // Check if a test plan already exists
        var existingTestPlan = await dbContext.TestPlans
            .Include(tp => tp.LinkedTestCases)
            .FirstOrDefaultAsync(tp => tp.Projects == projects && tp.Name == name);

        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        if (existingTestPlan != null)
        {
            // If the name has changed, create a new test plan
            if (existingTestPlan.Name != name)
            {
                var newTestPlan = new TestPlans
                {
                    Name = name,
                    Description = description,
                    Projects = projects,
                    CreatedBy = USER,
                    AssignedTo = assignedUserId,
                    WorkflowStatus = status
                };

                if (newTestPlan.WorkflowStatus == WorkflowStatus.Completed)
                    newTestPlan.ArchivedStatus = ArchivedStatus.Archived;

                await dbContext.TestPlans.AddAsync(newTestPlan);
                await dbContext.SaveChangesAsync();

                if (testCase != null)
                {
                    newTestPlan.LinkedTestCases.Add(testCase);
                    await dbContext.SaveChangesAsync();
                }

                return newTestPlan;
            }
            else
            {
                if (testCase != null && !existingTestPlan.LinkedTestCases.Any(tc => tc.Id == testCase.Id))
                {
                    existingTestPlan.LinkedTestCases.Add(testCase);
                    await dbContext.SaveChangesAsync();
                }

                return existingTestPlan;
            }
        }

        // No existing test plan, create a new one
        var newTestPlanForCreation = new TestPlans
        {
            Name = name,
            Description = description,
            Projects = projects,
            CreatedBy = USER,
            AssignedTo = assignedUserId,
            WorkflowStatus = status,
            Cycle = cycle
        };

        if (newTestPlanForCreation.WorkflowStatus == WorkflowStatus.Completed)
            newTestPlanForCreation.ArchivedStatus = ArchivedStatus.Archived;

        await dbContext.TestPlans.AddAsync(newTestPlanForCreation);
        await dbContext.SaveChangesAsync();

        if (testCase != null)
        {
            newTestPlanForCreation.LinkedTestCases.Add(testCase);
            await dbContext.SaveChangesAsync();
        }

        return newTestPlanForCreation;
    }


    private static async Task<Cycles> GetOrCreateCycle(ApplicationDbContext dbContext, Projects projects, string name)
    {
        // Check if a cycle already exists
        var existingCycle = await dbContext.Cycles
            .FirstOrDefaultAsync(c => c.Projects == projects && c.Name == name);
        if (existingCycle != null) return existingCycle;
        ;
        var newCycle = new Cycles
        {
            Name = name,
            Projects = projects,
            CreatedBy = USER
        };
        await dbContext.Cycles.AddAsync(newCycle);
        await dbContext.SaveChangesAsync();
        return newCycle;
    }


    private static async Task<TestExecution> GetOrCreateTestExecutionAsync(
        ApplicationDbContext dbContext,
        Projects projects,
        string testPlanName,
        string name)
    {
        // Check if test execution already exists
        var existingExecution = await dbContext.TestExecution
            .FirstOrDefaultAsync(te => te.Name == name && te.Projects == projects);

        if (existingExecution != null)
            return existingExecution;

        // Load the test plan with linked test cases
        var testPlan = await dbContext.TestPlans
            .Include(t => t.LinkedTestCases)
            .FirstOrDefaultAsync(t => t.Name == testPlanName && t.Projects == projects);

        if (testPlan == null)
            throw new InvalidOperationException($"Test plan '{testPlanName}' not found for project '{projects.Id}'.");

        // Create TestCaseExecutions for each linked TestCase
        var testCaseExecutions = testPlan.LinkedTestCases.Select(tc => new TestCaseExecution
        {
            TestCaseId = tc.Id,
            ExecutionStatus = ExecutionStatus.NotRun,
            CreatedBy = USER,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow
        }).ToList();

        // Create the TestExecution entity
        var newTestExecution = new TestExecution
        {
            Name = name,
            Projects = projects,
            ProjectsId = projects.Id,
            TestPlan = testPlan,
            TestPlanId = testPlan.Id,
            LinkedTestCaseExecutions = testCaseExecutions,
            CreatedBy = USER,
            CreatedAt = DateTime.UtcNow,
            ModifiedAt = DateTime.UtcNow,
            WorkflowStatus = WorkflowStatus.New
        };

        await dbContext.TestExecution.AddAsync(newTestExecution);
        await dbContext.SaveChangesAsync();

        return newTestExecution;
    }
}