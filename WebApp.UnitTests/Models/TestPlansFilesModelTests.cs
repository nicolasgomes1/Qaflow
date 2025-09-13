using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;
using WebApp.UnitTests.Models.Helpers;

namespace WebApp.UnitTests.Models;

public class TestPlansFilesModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly TestPlansFilesModel rm;
    private readonly ProjectModel pm;
    private readonly TestPlansModel rm1;
    
    public TestPlansFilesModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        rm = fixture.ServiceProvider.GetRequiredService<TestPlansFilesModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        rm1 = fixture.ServiceProvider.GetRequiredService<TestPlansModel>();
    }
    
    private readonly List<IBrowserFile>? _files = [
        new TestBrowserFileImpl("test1.txt", 100),
        new TestBrowserFileImpl("test2.txt", 200),
        new TestBrowserFileImpl("test3.txt", 900)

    ];
    
    [Fact]
    public async Task TestPlansFilesModel_SaveFilesToDb()
    {
        var projects = await pm.GetProjects();
        var project = projects.FirstOrDefault(r => r.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var testplan = await db.TestPlans.Where(x => x.Name == "Test Plan Alpha").Include(t => t.TestPlansFiles).FirstOrDefaultAsync();
        if (testplan == null) throw new Exception("Test Plan not found");
        var testplanById = await rm1.GetTestPlanByIdAsync(testplan.Id);

        // Count files before saving
        var fileCountBefore = await db.TestPlansFiles.CountAsync(f => f.TestPlanId == testplanById.Id);

        await rm.SaveFilesToDb(_files, testplanById.Id, project.Id);

        // Verify files were added
        var savedFiles = await db.TestPlansFiles
            .Where(f => f.TestPlanId == testplanById.Id)
            .ToListAsync();

        // Assert the number of files increased by the expected amount
        Assert.Equal(fileCountBefore + _files.Count, savedFiles.Count);

        // Verify file names were saved correctly
        foreach (var expectedFile in _files)
        {
            Assert.Contains(savedFiles, f => f.FileName == expectedFile.Name);
        }
        
        var filesAfter = await rm.GetFilesByTestPlanId(testplanById.Id, project.Id);
        Assert.Equal(3, filesAfter.Count);
    }
}