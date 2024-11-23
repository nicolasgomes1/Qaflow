using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Services.TestData;

public class ProjectDataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
        await using var
            dbContext =
                await dbContextFactory.CreateDbContextAsync(cancellationToken); // Use using for context disposal

        // Seeding process
        await SeedDataAsync(dbContext);
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
            new TestSteps
            {
                Number = 1,
                Description = "Step 1 Description",
                ExpectedResult = "Step 1 Expected Result"
            },
        };


        // Create or get project
        var project = await GetOrCreateProjectAsync(dbContext, "Demo Project");
        await GetOrCreateProjectAsync(dbContext, "Demo Project Without Data");


        // Create or get requirement
        var requirement = await GetOrCreateRequirementAsync(dbContext, project.Id, "Requirement A", "requirement is A");
        await GetOrCreateRequirementAsync(dbContext, project.Id, "Requirement B", "requirement is B");

        for (var i = 1; i <= 100; i++)
        {
            await GetOrCreateRequirementAsync(dbContext, project.Id, $"Requirement {i}", $"requirement number is {i}");
        }

        // Create or get test cases
        var testCase1 =
            await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 1", "Sample test case 1", null);
        var testCase2 =
            await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 2", "Sample test case 2", null);
        await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 3", "Sample test case ", requirement);

        for (var i = 1; i <= 100; i++)
        {
            await GetOrCreateTestCaseAsync(dbContext, project.Id, $"Test Case {i}.", $"Test Case number is {i}.", null);
        }

        await GetOrCreateTestCaseWithStepsAsync(dbContext, project.Id, "Test Case 4", "Sample test case 4", null,
            testSteps);

        // Create or get test plans associated with the test cases
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Alpha", testCase1);
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Alpha", testCase2);
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Beta", null);

        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 1", "Bug 1 Description");
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 2", "Bug 2 Description", BugStatus.Closed);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 3", "Bug 3 Description", BugStatus.InProgress);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 4", "Bug 4 Description", BugStatus.InReview);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 5", "Bug 5 Description", BugStatus.Open);
    }

    private static async Task<Projects> GetOrCreateProjectAsync(ApplicationDbContext dbContext, string projectName)
    {
        var existingProject = await dbContext.Projects.FirstOrDefaultAsync(p => p.Name == projectName);
        if (existingProject != null)
        {
            return existingProject; // Return existing project
        }

        // Create a new project if it doesn't exist
        var newProject = new Projects
        {
            Name = projectName,
            Description = "Project A Description",
            CreatedBy = "user@example.com"
        };

        await dbContext.Projects.AddAsync(newProject);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newProject;
    }

    private static async Task<Requirements> GetOrCreateRequirementAsync(ApplicationDbContext dbContext, int projectId,
        string name, string description)
    {
        var existingRequirement = await dbContext.Requirements
            .FirstOrDefaultAsync(r => r.RProjectId == projectId && r.Name == name);

        if (existingRequirement != null)
        {
            if (existingRequirement.Description != description)
            {
                existingRequirement.Description = description;
            }

            await dbContext.SaveChangesAsync();
            return existingRequirement;
        }

        // Create new requirement if it doesn't exist
        var newRequirement = new Requirements
        {
            Name = name,
            Description = description,
            RProjectId = projectId,
            CreatedBy = "user@example.com"
        };

        await dbContext.Requirements.AddAsync(newRequirement);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newRequirement;
    }


    private static async Task<Bugs> GetOrCreateBugsAsync(ApplicationDbContext dbContext, int projectId, string name,
        string description, BugStatus status = BugStatus.Open)
    {
        var existingBug = await dbContext.Bugs
            .FirstOrDefaultAsync(b => b.BProjectId == projectId && b.Name == name);

        if (existingBug != null)
        {
            return existingBug;
        }

        var newBug = new Bugs
        {
            Name = name,
            Description = description,
            BProjectId = projectId,
            CreatedBy = "user@example.com",
            BugStatus = status
        };

        await dbContext.Bugs.AddAsync(newBug);
        await dbContext.SaveChangesAsync();
        return newBug;
    }


    private static async Task<TestCases> GetOrCreateTestCaseAsync(ApplicationDbContext dbContext, int projectId,
        string name, string description, Requirements? requirements)
    {
        // Check if the test case already exists
        var existingTestCase = await dbContext.TestCases
            .Include(tc => tc.Requirements) // Include Requirements to avoid lazy loading
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.TcProjectId == projectId);

        if (existingTestCase != null)
        {
            return existingTestCase; // Return existing test case
        }

        // Create new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            TcProjectId = projectId,
            CreatedBy = "user@example.com"
        };

        if (requirements != null)
        {
            if (newTestCase.Requirements == null)
                throw new InvalidOperationException("Requirements collection is not initialized");

            newTestCase.Requirements.Add(requirements); // Add the requirement if it's provided
        }

        await dbContext.TestCases.AddAsync(newTestCase);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newTestCase;
    }

    private static async Task<TestCases> GetOrCreateTestCaseWithStepsAsync(
        ApplicationDbContext dbContext,
        int projectId,
        string name,
        string description,
        Requirements? requirements,
        List<TestSteps>? testStepsList // Accept a list of test steps
    )
    {
        // Check if the test case already exists
        var existingTestCase = await dbContext.TestCases
            .AsSplitQuery()
            .Include(tc => tc.Requirements) // Include Requirements to avoid lazy loading
            .Include(tc => tc.TestSteps) // Include TestSteps to avoid lazy loading
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.TcProjectId == projectId);

        if (existingTestCase != null)
        {
            return existingTestCase; // Return existing test case
        }

        // Create a new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            TcProjectId = projectId,
            CreatedBy = "user@example.com"
        };

        // Add the requirement if it's provided
        if (requirements != null)
        {
            if (newTestCase.Requirements == null)
                throw new InvalidOperationException("Requirements collection is not initialized");

            newTestCase.Requirements.Add(requirements);
        }

        // Add test steps if the list is not null and has elements
        if (testStepsList != null && testStepsList.Any())
        {
            if (newTestCase.TestSteps == null)
                throw new InvalidOperationException("TestSteps collection is not initialized");

            foreach (var step in testStepsList)
            {
                step.TestCaseId = newTestCase.Id; // Link step to this test case by setting the foreign key
                newTestCase.TestSteps.Add(step); // Add the step to the test case's TestSteps collection
            }
        }

        await dbContext.TestCases.AddAsync(newTestCase);
        await dbContext.SaveChangesAsync(); // Save to get the Id and link TestSteps

        return newTestCase;
    }


    private static async Task GetOrCreateTestPlanAsync(ApplicationDbContext dbContext, int projectId, string name,
        TestCases? testCase)
    {
        // Check if a test plan already exists
        var existingTestPlan = await dbContext.TestPlans
            .Include(tp => tp.TestCases) // Include test cases to avoid lazy loading
            .FirstOrDefaultAsync(tp => tp.TPProjectId == projectId && tp.Name == name);

        if (existingTestPlan != null)
        {
            // If the name has changed, create a new test plan
            if (existingTestPlan.Name != name)
            {
                // Create a new test plan with the updated name
                var newTestPlan = new TestPlans
                {
                    Name = name,
                    TPProjectId = projectId,
                    CreatedBy = "user@example.com"
                };

                await dbContext.TestPlans.AddAsync(newTestPlan);
                await dbContext.SaveChangesAsync();

                if (testCase == null) return;
                newTestPlan.TestCases.Add(testCase);
                await dbContext.SaveChangesAsync();
            }
            else
            {
                if (testCase == null || existingTestPlan.TestCases.Any(tc => tc.Id == testCase.Id))
                    return;
                existingTestPlan.TestCases.Add(testCase);
                await dbContext.SaveChangesAsync();
            }

            return;
        }


        var newTestPlanForCreation = new TestPlans
        {
            Name = name,
            TPProjectId = projectId,
            CreatedBy = "user@example.com"
        };

        await dbContext.TestPlans.AddAsync(newTestPlanForCreation);
        await dbContext.SaveChangesAsync(); // Save to get the Id

        // If a test case is provided, associate it with the new test plan
        if (testCase != null)
        {
            newTestPlanForCreation.TestCases.Add(testCase);
            await dbContext.SaveChangesAsync(); // Save the association
        }
    }
}