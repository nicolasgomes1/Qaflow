using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(ProjectModel))]
public class ProjectModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext _db;
    private readonly ProjectModel _pm;

    public ProjectModelTests(TestFixture fixture)
    {
        _db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
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
        var initialCount = await _db.Projects.CountAsync();

        // Add a new project
        await _pm.AddProject(newProject);

        // Check that the project count increased by 1
        var finalCount = await _db.Projects.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task ProjectModel_GetProjects()
    {
        var initialCount = await _db.Projects.CountAsync();

        // Adding multiple projects
        for (var i = 0; i < 3; i++)
        {
            var newProject = new Data.Projects
            {
                Name = $"sample{i}",
                Description = $"Description{i}"
            };

            await _pm.AddProject(newProject);
        }

        var finalCount = await _db.Projects.CountAsync();

        // Verify the number of projects is exactly 3
        var result = await _pm.GetProjects();
        //      Assert.Equal(initialCount+3, result.Count());
        Assert.Equal(initialCount + 3, finalCount);
    }

    [Fact]
    public async Task ProjectModel_RemoveProject()
    {
        var initialCount = await _db.Projects.CountAsync();

        // First, add a project
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await _pm.AddProject(newProject);

        var count = await _db.Projects.CountAsync();
        // Ensure it was added
        Assert.Equal(initialCount + 1, count);
        // Remove the project
        await _pm.RemoveProject(newProject.Id);

        // Ensure the count decreased by 1 (check relative change)
        var finalCount = await _db.Projects.CountAsync();
        Assert.Equal(initialCount - 1, finalCount - 1);
    }

    [Fact]
    public async Task ProjectModel_RemoveNonExistentProject()
    {
        // Try removing a project that doesn't exist
        var initialCount = await _db.Projects.CountAsync();

        // Attempt to remove a non-existent project
        await Assert.ThrowsAsync<Exception>(() => _pm.RemoveProject(0)); // Random non-existent Id        
        // The count should remain unchanged (no side effects)
        var finalCount = await _db.Projects.CountAsync();
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

        await _pm.AddProject(newProject);

        var createdProject = await _pm.GetProjectById(newProject.Id);
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

        await _pm.AddProject(newProject);

        // Retrieve the newly added project and validate
        var addedProject = await _pm.GetProjectById(newProject.Id);
        Assert.NotNull(addedProject);
        Assert.Equal("sample", addedProject.Name);
        Assert.Equal("Description", addedProject.Description);

        addedProject.Name = "updated sample";
        await _pm.UpdateProject(addedProject);


        // Retrieve the updated project
        var updatedProject = await _pm.GetProjectById(addedProject.Id);

        // Assert that the name was updated
        Assert.Equal("updated sample", updatedProject.Name);
    }
}