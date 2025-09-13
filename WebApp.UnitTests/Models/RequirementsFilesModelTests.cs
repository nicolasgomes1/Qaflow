using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;
using WebApp.UnitTests.Models.Helpers;

namespace WebApp.UnitTests.Models;

public class RequirementsFilesModelTests  : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly RequirementsFilesModel rm;
    private readonly ProjectModel pm;
    private readonly RequirementsModel rm1;
    
    public RequirementsFilesModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        rm = fixture.ServiceProvider.GetRequiredService<RequirementsFilesModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        rm1 = fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
    }
    
    private readonly List<IBrowserFile>? _files = [
        new TestBrowserFileImpl("test1.txt", 100),
        new TestBrowserFileImpl("test2.txt", 200)
    ];
    
    [Fact]
    public async Task RequiremlentsFilesModel_SaveFilesToDb()
    {
        var projects = await pm.GetProjects();
        var project = projects.FirstOrDefault(r => r.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var requirement = await db.Requirements.Where(x => x.Name == "Requirement A").Include(t => t.LinkedRequirementsFiles).FirstOrDefaultAsync();
        if (requirement == null) throw new Exception("Requirement not found");
        var requirementById = await rm1.GetRequirementByIdAsync(requirement.Id);

        // Count files before saving
        var fileCountBefore = await db.RequirementsFiles.CountAsync(f => f.RequirementsId == requirementById.Id);

        await rm.SaveFilesToDb(_files, requirementById.Id, project.Id);

        // Verify files were added
        var savedFiles = await db.RequirementsFiles
            .Where(f => f.RequirementsId == requirementById.Id)
            .ToListAsync();

        // Assert the number of files increased by the expected amount
        Assert.Equal(fileCountBefore + _files.Count, savedFiles.Count);

        // Verify file names were saved correctly
        foreach (var expectedFile in _files)
        {
            Assert.Contains(savedFiles, f => f.FileName == expectedFile.Name);
        }
        
        var filesAfter = await rm.GetFilesByRequirementId(requirementById.Id);
        Assert.Equal(2, filesAfter.Count);
    }
}