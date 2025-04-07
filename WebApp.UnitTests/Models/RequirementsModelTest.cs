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
    private readonly ApplicationDbContext db;
    private readonly RequirementsModel rm;
    private readonly ProjectModel pm;
    private readonly RequirementsFilesModel rfm;

    public RequirementsModelTest(TestFixture fixture)
    {
        // Resolve services via ServiceProvider
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        rm = fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        rfm = fixture.ServiceProvider.GetRequiredService<RequirementsFilesModel>();
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

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;

        await rm.UpdateRequirement(newRequirement, null, project.Id);
        // db.Requirements.Update(newRequirement);
        //await db.SaveChangesAsync();

        var updatedRequirement = await rm.GetRequirementByIdAsync(newRequirement.Id);
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

        await rm.AddRequirement(newRequirement, null, project.Id);

        var requirementspec = await db.RequirementsSpecification.Where(x => x.Name == "Requirements Specification 3")
            .FirstOrDefaultAsync();

        if (requirementspec == null) throw new Exception("Requirements Specification 3 not found");
        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;
        newRequirement.RequirementsSpecificationId = requirementspec.Id;

        await rm.UpdateRequirement(newRequirement, null, project.Id);


        var updatedRequirement = await rm.GetRequirementByIdAsync(newRequirement.Id);
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
        var requirementSpec = await db.RequirementsSpecification.Where(x => x.Name == "Requirements Specification 3")
            .FirstOrDefaultAsync();
        if (requirementSpec == null) throw new Exception("Requirements Specification 3 not found");

        var newRequirement = new Requirements
        {
            Name = "original",
            Description = "original description",
            ProjectsId = project.Id,
            RequirementsSpecificationId = requirementSpec.Id // Requirements Specification 1 is deleted in the database
        };

        await rm.AddRequirement(newRequirement, null, project.Id);


        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";

        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;
        newRequirement.RequirementsSpecificationId = -1;

        await rm.UpdateRequirement(newRequirement, null, project.Id);


        var updatedRequirement = await rm.GetRequirementByIdAsync(newRequirement.Id);
        Assert.Equal("updated", updatedRequirement.Name);
        Assert.Equal("updated description", updatedRequirement.Description);
        Assert.Equal(Priority.Critical, updatedRequirement.Priority);
        Assert.Equal("admin@example.com", updatedRequirement.AssignedTo);
        Assert.Equal(WorkflowStatus.New, updatedRequirement.WorkflowStatus);
        Assert.Equal(ArchivedStatus.Active, updatedRequirement.ArchivedStatus);
        Assert.Equal(-1,
            db.Requirements.Where(x => x.Id == updatedRequirement.Id).Select(x => x.RequirementsSpecificationId)
                .FirstOrDefault());
        Assert.DoesNotContain(db.Requirements.Where(x => x.Id == updatedRequirement.Id),
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

        await rm.AddRequirement(newRequirement, browserFiles, project.Id);

        newRequirement.Name = "updated";
        newRequirement.Description = "updated description";
        newRequirement.Priority = Priority.Critical;
        newRequirement.AssignedTo = "admin@example.com";
        newRequirement.WorkflowStatus = WorkflowStatus.New;
        newRequirement.ArchivedStatus = ArchivedStatus.Active;

        await rm.UpdateRequirement(newRequirement, null, project.Id);


        var requirementFiles = db.RequirementsFiles.Where(x => x.RequirementsId == newRequirement.Id).ToList();

        Assert.Equal(2, requirementFiles.Count);
        var updatedRequirement = await rm.GetRequirementByIdAsync(newRequirement.Id);
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

        await rfm.SaveFilesToDb(browserFiles, newRequirement.Id, project.Id);
        await rm.AddRequirement(newRequirement, browserFiles, project.Id);
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

        await rfm.SaveFilesToDb(browserFiles, newRequirement.Id, project.Id);
        await rm.AddRequirement(newRequirement, browserFiles, project.Id);

        var files = await rfm.GetFilesByRequirementId(newRequirement.Id);
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