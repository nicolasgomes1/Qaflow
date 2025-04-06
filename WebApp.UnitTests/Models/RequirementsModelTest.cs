using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(RequirementsModel))]
public class RequirementsModelTest
{
    private readonly ApplicationDbContext db;
    private readonly RequirementsModel rm;
    private readonly ProjectModel pm;

    public RequirementsModelTest(TestFixture fixture)
    {
        // Resolve services via ServiceProvider
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        rm = fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
    }

    private async Task<int> GetRequirementCountAsync()
    {
        return await db.Requirements.CountAsync();
    }

    private async Task<Data.Projects> CreateAndSetProject()
    {
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await pm.AddProject(newProject);
        var project = newProject;

        return project;
    }

    [Fact]
    public async Task RequirementsModel_AddRequirement()
    {
        var project = await CreateAndSetProject();

        var newRequirement = new Requirements
        {
            Name = "sample",
            Description = "Description",
            ProjectsId = project.Id
        };

        var initialCount = await GetRequirementCountAsync();

        await rm.AddRequirement(newRequirement, null, project.Id);

        var finalCount = await GetRequirementCountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task RequirementsModel_UpdateRequirement()
    {
        var project = await CreateAndSetProject();


        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id
        };

        await rm.AddRequirement(newRequirement, null, project.Id);

        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        await rm.UpdateRequirement(newRequirement.Id, null, project.Id);

        var updatedRequirement = await rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
    }
}