using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;
using WebApp.UnitTests.Models.Helpers;

namespace WebApp.UnitTests.Models;

public class BugsFilesModelTests  : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly BugsFilesModel bm;
    private readonly ProjectModel pm;
    private readonly BugsModel bm1;

    public BugsFilesModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        bm = fixture.ServiceProvider.GetRequiredService<BugsFilesModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        bm1 = fixture.ServiceProvider.GetRequiredService<BugsModel>();
    }

    private readonly List<IBrowserFile>? _files = [
        new TestBrowserFileImpl("test1.txt", 100),
        new TestBrowserFileImpl("test2.txt", 200)
    ];


    [Fact]
    public async Task BugsFilesModel_SaveFilesToDb()
    {
        var projects = await pm.GetProjects();
        var project = projects.FirstOrDefault(r => r.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var bug = await db.Bugs.Where(x => x.Name == "Bug 1").Include(bugs => bugs.BugFiles).FirstOrDefaultAsync();
        if (bug == null) throw new Exception("Bug not found");
        var bugById = await bm1.GetBugByIdAsync(bug.Id);

        // Count files before saving
        var fileCountBefore = await db.BugsFiles.CountAsync(f => f.BugId == bugById.Id);

        await bm.SaveFilesToDb(_files, bugById.Id, project.Id);

        // Verify files were added
        var savedFiles = await db.BugsFiles
            .Where(f => f.BugId == bugById.Id)
            .ToListAsync();

        // Assert the number of files increased by the expected amount
        Assert.Equal(fileCountBefore + _files!.Count, savedFiles.Count);

        // Verify file names were saved correctly
        foreach (var expectedFile in _files)
        {
            Assert.Contains(savedFiles, f => f.FileName == expectedFile.Name);
        }
        
        var filesAfter = await bm.GetBugFilesById(bugById.Id);
        Assert.Equal(2, filesAfter.Count);
    }


}


