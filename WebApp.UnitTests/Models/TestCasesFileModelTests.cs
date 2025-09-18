using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;
using WebApp.UnitTests.Models.Helpers;

namespace WebApp.UnitTests.Models;

public class TestCasesFileModelTests : IClassFixture<TestFixture>
{
    private readonly List<IBrowserFile>? _files =
    [
        new TestBrowserFileImpl("test1.txt", 100),
        new TestBrowserFileImpl("test2.txt", 200)
    ];

    private readonly ApplicationDbContext db;
    private readonly ProjectModel pm;
    private readonly TestCasesFilesModel tm;
    private readonly TestCasesModel tm1;

    public TestCasesFileModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        tm = fixture.ServiceProvider.GetRequiredService<TestCasesFilesModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        tm1 = fixture.ServiceProvider.GetRequiredService<TestCasesModel>();
    }

    [Fact]
    public async Task TestCasesFilesModel_SaveFilesToDb()
    {
        var projects = await pm.GetProjects();
        var project = projects.FirstOrDefault(r => r.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var testcase = await db.TestCases.Where(x => x.Name == "Test Case 1").Include(t => t.LinkedTestCasesFiles)
            .FirstOrDefaultAsync();
        if (testcase == null) throw new Exception("Test Case not found");
        var testcaseById = await tm1.GetTestCasesByIdAsync(testcase.Id);

        // Count files before saving
        var fileCountBefore = await db.TestCasesFiles.CountAsync(f => f.TestCaseId == testcaseById.Id);

        await tm.SaveFilesToDb(_files, testcaseById.Id, project.Id);

        // Verify files were added
        var savedFiles = await db.TestCasesFiles
            .Where(f => f.TestCaseId == testcaseById.Id)
            .ToListAsync();

        // Assert the number of files increased by the expected amount
        Assert.Equal(fileCountBefore + _files!.Count, savedFiles.Count);

        // Verify file names were saved correctly
        foreach (var expectedFile in _files) Assert.Contains(savedFiles, f => f.FileName == expectedFile.Name);

        var filesAfter = await tm.GetFilesByTestCaseId(testcaseById.Id);
        Assert.Equal(2, filesAfter.Count);
    }
}