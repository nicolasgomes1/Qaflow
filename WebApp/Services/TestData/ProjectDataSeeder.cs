using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Services.TestData;

public class ProjectDataSeeder(IServiceProvider serviceProvider) : IHostedService
{
    const string USER = "user@example.com";
    const string MANAGER = "manager@example.com";
    private const string ADMIN = "admin@example.com";
    
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
        var requirement = await GetOrCreateRequirementAsync(dbContext, project.Id, "Requirement A", "requirement is A", USER);
        await GetOrCreateRequirementAsync(dbContext, project.Id, "Requirement B", "requirement is B",MANAGER);

        for (var i = 1; i <= 100; i++)
        {
            await GetOrCreateRequirementAsync(dbContext, project.Id, $"Requirement {i}", $"requirement number is {i}", MANAGER);
        }

        // Create or get test cases
        var testCase1 =
            await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 1", "Sample test case 1", USER, null);
        var testCase2 =
            await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 2", "Sample test case 2", MANAGER, null);
        await GetOrCreateTestCaseAsync(dbContext, project.Id, "Test Case 3", "Sample test case ", USER, requirement);

        for (var i = 1; i <= 100; i++)
        {
            await GetOrCreateTestCaseAsync(dbContext, project.Id, $"Test Case {i}.", $"Test Case number is {i}.", USER, null);
        }

        await GetOrCreateTestCaseWithStepsAsync(dbContext, project.Id, "Test Case 4", "Sample test case 4", null,
            testSteps);

        // Create or get test plans associated with the test cases
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Alpha", USER, testCase1);
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Alpha", MANAGER, testCase2);
        await GetOrCreateTestPlanAsync(dbContext, project.Id, "Test Plan Beta", USER, null);

        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 1", "Bug 1 Description", USER);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 2", "Bug 2 Description", USER, BugStatus.Closed);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 3", "Bug 3 Description", USER, BugStatus.InProgress);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 4", "Bug 4 Description", MANAGER, BugStatus.InReview);
        await GetOrCreateBugsAsync(dbContext, project.Id, "Bug 5", "Bug 5 Description", MANAGER, BugStatus.Open);
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
            CreatedBy = USER
        };

        await dbContext.Projects.AddAsync(newProject);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newProject;
    }

    private static async Task<Requirements> GetOrCreateRequirementAsync(ApplicationDbContext dbContext, int projectId,
        string name, string description, string assignedUserName)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        // Check if the requirement already exists
        var existingRequirement = await dbContext.Requirements
            .FirstOrDefaultAsync(r => r.ProjectsId == projectId && r.Name == name);

        if (existingRequirement != null)
        {
            bool isModified = false;

            // Update description if it has changed
            if (existingRequirement.Description != description)
            {
                existingRequirement.Description = description;
                isModified = true;
            }

            // Update AssignedTo if it has changed
            if (existingRequirement.AssignedTo != assignedUserId)
            {
                existingRequirement.AssignedTo = assignedUserId;
                isModified = true;
            }

            if (isModified)
            {
                await dbContext.SaveChangesAsync();
            }

            return existingRequirement;
        }

        // Create new requirement if it doesn't exist
        var newRequirement = new Requirements
        {
            Name = name,
            Description = description,
            ProjectsId = projectId,
            AssignedTo = assignedUserId,
            CreatedBy = assignedUserName
        };

        await dbContext.Requirements.AddAsync(newRequirement);
        await dbContext.SaveChangesAsync(); // Save to get the Id
        return newRequirement;
    }

    private static async Task<string> AssignedUserId(ApplicationDbContext dbContext, string assignedUserName)
    {
        // Retrieve the user Id for the assigned user
        var assignedUser = await dbContext.Users
            .FirstOrDefaultAsync(u => u.UserName == assignedUserName);

        if (assignedUser == null)
        {
            throw new InvalidOperationException($"User with UserName '{assignedUserName}' not found.");
        }

        var assignedUserId = assignedUser.Id;
        return assignedUserId;
    }


    private static async Task<Bugs> GetOrCreateBugsAsync(ApplicationDbContext dbContext, int projectId, string name,
        string description, string assignedUserName, BugStatus status = BugStatus.Open)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        
        var existingBug = await dbContext.Bugs
            .FirstOrDefaultAsync(b => b.ProjectsId == projectId && b.Name == name);

        if (existingBug != null)
        {
            return existingBug;
        }

        var newBug = new Bugs
        {
            Name = name,
            Description = description,
            ProjectsId = projectId,
            CreatedBy = USER,
            BugStatus = status,
            AssignedTo = assignedUserId
        };

        await dbContext.Bugs.AddAsync(newBug);
        await dbContext.SaveChangesAsync();
        return newBug;
    }


    private static async Task<TestCases> GetOrCreateTestCaseAsync(ApplicationDbContext dbContext, int projectId,
        string name, string description, string assignedUserName, Requirements? requirements)
    {
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);

        // Check if the test case already exists
        var existingTestCase = await dbContext.TestCases
            .Include(tc => tc.Requirements) // Include Requirements to avoid lazy loading
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.ProjectsId == projectId);

        if (existingTestCase != null)
        {
            return existingTestCase; // Return existing test case
        }

        // Create new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            ProjectsId = projectId,
            CreatedBy = USER,
            AssignedTo = assignedUserId,
            WorkflowStatus = WorkflowStatus.Completed

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
            .FirstOrDefaultAsync(tc => tc.Name == name && tc.ProjectsId == projectId);

        if (existingTestCase != null)
        {
            return existingTestCase; // Return existing test case
        }

        // Create a new test case if it doesn't exist
        var newTestCase = new TestCases
        {
            Name = name,
            Description = description,
            ProjectsId = projectId,
            CreatedBy = USER,
            WorkflowStatus = WorkflowStatus.Completed
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
                step.TestCasesId = newTestCase.Id; // Link step to this test case by setting the foreign key
                newTestCase.TestSteps.Add(step); // Add the step to the test case's TestSteps collection
            }
        }

        await dbContext.TestCases.AddAsync(newTestCase);
        await dbContext.SaveChangesAsync(); // Save to get the Id and link TestSteps

        return newTestCase;
    }


    private static async Task GetOrCreateTestPlanAsync(ApplicationDbContext dbContext, int projectId, string name, string assignedUserName,
        TestCases? testCase)
    {
        
        // Check if a test plan already exists
        var existingTestPlan = await dbContext.TestPlans
            .Include(tp => tp.TestCases) // Include test cases to avoid lazy loading
            .FirstOrDefaultAsync(tp => tp.ProjectsId == projectId && tp.Name == name);
        
        var assignedUserId = await AssignedUserId(dbContext, assignedUserName);


        if (existingTestPlan != null)
        {
            // If the name has changed, create a new test plan
            if (existingTestPlan.Name != name)
            {
                // Create a new test plan with the updated name
                var newTestPlan = new TestPlans
                {
                    Name = name,
                    ProjectsId = projectId,
                    CreatedBy = USER,
                    AssignedTo = assignedUserId
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
            ProjectsId = projectId,
            CreatedBy = USER,
            AssignedTo = assignedUserId
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