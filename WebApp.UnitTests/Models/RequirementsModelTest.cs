using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(RequirementsModel))]
public class RequirementsModelTest : TestBase
{
    private readonly TestFixture _fixture;
    private readonly ApplicationDbContext _db;
    private readonly RequirementsModel _rm;
    private readonly ProjectStateService _ps;
    private readonly ProjectModel _pm;

    public RequirementsModelTest(TestFixture fixture) : base(fixture)
    {
        _fixture = fixture;

        // Resolve services via ServiceProvider
        _db = _fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _rm = _fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
        _ps = _fixture.ServiceProvider.GetRequiredService<ProjectStateService>();
        _pm = _fixture.ServiceProvider.GetRequiredService<ProjectModel>();
    }

    private async Task<int> GetRequirementCountAsync()
    {
        return await _db.Requirements.CountAsync();
    }

    private async Task<Data.Projects> CreateAndSetProject()
    {
        var newProject = new Data.Projects
        {
            Name = "sample",
            Description = "Description"
        };

        await _pm.AddProject(newProject);
        var project = newProject;

        _ps.SetProjectId(project.Id);
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

        await _rm.AddRequirement(newRequirement, null, project.Id);

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

        await _rm.AddRequirement(newRequirement, null, project.Id);

        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        await _rm.UpdateRequirement(newRequirement.Id, null, project.Id);

        var updatedRequirement = await _rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
    }
}