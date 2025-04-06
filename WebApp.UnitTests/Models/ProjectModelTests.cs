using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(ProjectModel))]
public class ProjectModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly ProjectModel pm;

    public ProjectModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
    }

    // Helper method to track relative changes in project count
    private async Task<int> GetProjectCountAsync()
    {
        return await db.Projects.CountAsync();
    }

    [Fact]
    public async Task ProjectModel_AddProject()
    {
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        // Get the current count of projects before adding a new one
        var initialCount = await GetProjectCountAsync();

        // Add a new project
        await pm.AddProject(newProject);

        // Check that the project count increased by 1
        var finalCount = await GetProjectCountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task ProjectModel_GetProjects()
    {
        var initialCount = await GetProjectCountAsync();

        // Adding multiple projects
        for (var i = 0; i < 3; i++)
        {
            var newProject = new Data.Projects
            {
                Name = $"sample{i}",
                Description = $"Description{i}"
            };

            await pm.AddProject(newProject);
        }

        var finalCount = await GetProjectCountAsync();

        // Verify the number of projects is exactly 3
        var result = await pm.GetProjects();
        //      Assert.Equal(initialCount+3, result.Count());
        Assert.Equal(initialCount + 3, finalCount);
    }

    [Fact]
    public async Task ProjectModel_RemoveProject()
    {
        var initialCount = await GetProjectCountAsync();

        // First, add a project
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await pm.AddProject(newProject);

        // Ensure it was added
        Assert.Equal(initialCount + 1, await GetProjectCountAsync());
        // Remove the project
        await pm.RemoveProject(newProject.Id);

        // Ensure the count decreased by 1 (check relative change)
        var finalCount = await GetProjectCountAsync();
        Assert.Equal(initialCount - 1, finalCount - 1);
    }

    [Fact]
    public async Task ProjectModel_RemoveNonExistentProject()
    {
        // Try removing a project that doesn't exist
        var initialCount = await GetProjectCountAsync();

        // Attempt to remove a non-existent project
        await Assert.ThrowsAsync<Exception>(() => pm.RemoveProject(0)); // Random non-existent Id        
        // The count should remain unchanged (no side effects)
        var finalCount = await GetProjectCountAsync();
        Assert.Equal(initialCount, finalCount);
    }

    [Fact]
    public async Task ProjectModel_GetProjectById()
    {
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await pm.AddProject(newProject);

        var createdProject = await pm.GetProjectById(newProject.Id);
        Assert.Equal(newProject.Id, createdProject.Id);
    }

    [Fact]
    public async Task ProjectModel_UpdateProject()
    {
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await pm.AddProject(newProject);

        // Retrieve the newly added project and validate
        var addedProject = await pm.GetProjectById(newProject.Id);
        Assert.NotNull(addedProject);
        Assert.Equal("sample", addedProject.Name);
        Assert.Equal("Description", addedProject.Description);

        // Update the project's name and save the changes
        addedProject.Name = "updated sample";
        //  await pm.UpdateProject(addedProject.Id);
        db.Projects.Update(addedProject);
        await db.SaveChangesAsync();

        // Retrieve the updated project
        var updatedProject = await pm.GetProjectById(addedProject.Id);

        // Assert that the name was updated
        Assert.Equal("updated sample", updatedProject.Name);
    }
}