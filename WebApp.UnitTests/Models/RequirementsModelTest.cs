using JetBrains.Annotations;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(RequirementsModel))]
public class RequirementsModelTest : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext _db;
    private readonly RequirementsModel _rm;
    private readonly ProjectModel _pm;
    private readonly RequirementsFilesModel _rfm;

    public RequirementsModelTest(TestFixture fixture)
    {
        // Resolve services via ServiceProvider
        _db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _rm = fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
        _pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        _rfm = fixture.ServiceProvider.GetRequiredService<RequirementsFilesModel>();
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

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;

        await _rm.UpdateRequirement(newRequirement, null, project.Id);
        // db.Requirements.Update(newRequirement);
        //await db.SaveChangesAsync();

        var updatedRequirement = await _rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
        Assert.Equal(Priority.Critical, updatedRequirement.Priority);
        Assert.Equal("admin@example.com", updatedRequirement.AssignedTo);
        Assert.Equal(WorkflowStatus.New, updatedRequirement.WorkflowStatus);
        Assert.Equal(ArchivedStatus.Active, updatedRequirement.ArchivedStatus);
    }

    [Fact]
    public async Task RequirementsModel_UpdateRequirementWithRequirementsSpecifications()
    {
        var project = await CreateAndSetProject();

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id
        };

        await _rm.AddRequirement(newRequirement, null, project.Id);

        var requirementspec = await _db.RequirementsSpecification.Where(x => x.Name == "Requirements Specification 3")
            .FirstOrDefaultAsync();

        if (requirementspec == null) throw new Exception("Requirements Specification 3 not found");
        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;
        newRequirement.RequirementsSpecificationId = requirementspec.Id;

        await _rm.UpdateRequirement(newRequirement, null, project.Id);


        var updatedRequirement = await _rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
        Assert.Equal(Priority.Critical, updatedRequirement.Priority);
        Assert.Equal("admin@example.com", updatedRequirement.AssignedTo);
        Assert.Equal(WorkflowStatus.New, updatedRequirement.WorkflowStatus);
        Assert.Equal(ArchivedStatus.Active, updatedRequirement.ArchivedStatus);
        Assert.Equal(requirementspec.Id, updatedRequirement.RequirementsSpecificationId);
    }

    [Fact]
    public async Task RequirementsModel_UpdateRequirementWithRequirementsSpecificationsDelete()
    {
        var project = await CreateAndSetProject();
        var requirementSpec = await _db.RequirementsSpecification.Where(x => x.Name == "Requirements Specification 3")
            .FirstOrDefaultAsync();
        if (requirementSpec == null) throw new Exception("Requirements Specification 3 not found");

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id,
            RequirementsSpecificationId = requirementSpec.Id // Requirements Specification 1 is deleted in the database
        };

        await _rm.AddRequirement(newRequirement, null, project.Id);


        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;
        newRequirement.RequirementsSpecificationId = -1;

        await _rm.UpdateRequirement(newRequirement, null, project.Id);


        var updatedRequirement = await _rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
        Assert.Equal(Priority.Critical, updatedRequirement.Priority);
        Assert.Equal("admin@example.com", updatedRequirement.AssignedTo);
        Assert.Equal(WorkflowStatus.New, updatedRequirement.WorkflowStatus);
        Assert.Equal(ArchivedStatus.Active, updatedRequirement.ArchivedStatus);
        Assert.Equal(-1,
            _db.Requirements.Where(x => x.Id == updatedRequirement.Id).Select(x => x.RequirementsSpecificationId)
                .FirstOrDefault());
        Assert.DoesNotContain(_db.Requirements.Where(x => x.Id == updatedRequirement.Id),
            x => x.RequirementsSpecification != null);
    }

    [Fact]
    public async Task RequirementsModel_AddRequirementWithFiles()
    {
        var project = await CreateAndSetProject();

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id
        };

        var bytes = new byte[] { 115, 116, 114, 105, 103, 110 };

        var browserFiles = new List<IBrowserFile>
        {
            new TestBrowserFile("test.txt", bytes),
            new TestBrowserFile("test1.txt", bytes)
        };

        await _rm.AddRequirement(newRequirement, browserFiles, project.Id);

        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";
        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;

        await _rm.UpdateRequirement(newRequirement, null, project.Id);


        var requirementFiles = _db.RequirementsFiles.Where(x => x.RequirementsId == newRequirement.Id).ToList();

        Assert.Equal(2, requirementFiles.Count);
        var updatedRequirement = await _rm.GetRequirementByIdAsync(newRequirement.Id);
    }

    [Fact]
    public async Task RequirementsModel_AddRequirementWithFilesUpdateFiles()
    {
        var project = await CreateAndSetProject();

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id
        };

        var bytes = new byte[] { 115, 116, 114, 105, 103, 110 };

        var browserFiles = new List<IBrowserFile>
        {
            new TestBrowserFile("test.txt", bytes),
            new TestBrowserFile("test1.txt", bytes)
        };

        await _rfm.SaveFilesToDb(browserFiles, newRequirement.Id, project.Id);
        await _rm.AddRequirement(newRequirement, browserFiles, project.Id);
    }

    [Fact]
    public async Task RequirementsModel_GetRequirementsFiles()
    {
        var project = await CreateAndSetProject();

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id
        };

        var bytes = new byte[] { 115, 116, 114, 105, 103, 110 };

        var browserFiles = new List<IBrowserFile>
        {
            new TestBrowserFile("test.txt", bytes),
            new TestBrowserFile("test1.txt", bytes)
        };

        await _rfm.SaveFilesToDb(browserFiles, newRequirement.Id, project.Id);
        await _rm.AddRequirement(newRequirement, browserFiles, project.Id);

        var files = await _rfm.GetFilesByRequirementId(newRequirement.Id);
        Assert.Equal(2, files.Count);
    }


    private class TestBrowserFile : IBrowserFile
    {
        public TestBrowserFile(string name, byte[] content)
        {
            Name = name;
            LastModified = DateTimeOffset.Now;
            ContentType = "text/plain";
            Size = content.Length;
            Content = content;
        }

        public string Name { get; }
        public DateTimeOffset LastModified { get; }
        public string ContentType { get; }
        public long Size { get; }
        public byte[] Content { get; }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            return new MemoryStream(Content);
        }
    }
}