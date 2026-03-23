using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

public class CyclesModelTests : IClassFixture<TestFixture>
{
    private readonly CyclesModel cm;
    private readonly ApplicationDbContext db;
    private readonly ProjectModel pm;

    public CyclesModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        cm = fixture.ServiceProvider.GetRequiredService<CyclesModel>();
        pm = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
    }


    [Fact]
    public async Task CyclesModel_GetCyclesAsync()
    {
        var project = await pm.GetProjectById(1);
        var cycle = await cm.GetCyclesByProjectId(project.Id);
        Assert.Equal(5, cycle.Count);
    }

    [Fact]
    public async Task CyclesModel_GetCycleAsync()
    {
        var project = await pm.GetProjectById(1);
        var cycles = await cm.GetCyclesByProjectId(project.Id);

        Assert.NotEmpty(cycles); // Verify we have any cycles

        // Get the first available cycle instead of looking for a specific name
        var selectedCycle = cycles.First();
        Assert.NotNull(selectedCycle);

        var dbcycle = await cm.GetCycleById(selectedCycle.Id);
        Assert.NotNull(dbcycle);
        Assert.Equal(selectedCycle.Id, dbcycle.Id);
    }

    [Fact]
    public async Task DeleteCycle()
    {
        var project = await pm.GetProjectById(1);
        var cycle = await cm.GetCyclesByProjectId(project.Id);
        var selectedCycle = cycle.Where(n => n.Name == "Cycle 2");
        if (selectedCycle is null) throw new Exception("Cycle not found");
        var cyclesEnumerable = selectedCycle as Cycles[] ?? selectedCycle.ToArray();
        await cm.DeleteCycle(cyclesEnumerable.First().Id);
        var deletedCycle = await cm.GetCycleById(cyclesEnumerable.First().Id);
        var dbcycle = db.Cycles?.FirstOrDefault(t => deletedCycle != null && t.Id == deletedCycle.Id);

        Assert.Null(dbcycle);
    }

    [Fact]
    public async Task DeleteCycleCantBeDeleted()
    {
        var project = await pm.GetProjectById(1);
        var cycle = await cm.GetCyclesByProjectId(project.Id);
        var selectedCycle = cycle.Where(n => n.Name == "Cycle 1");
        if (selectedCycle is null) throw new Exception("Cycle not found");
        var cyclesEnumerable = selectedCycle as Cycles[] ?? selectedCycle.ToArray();
        var isdeleted = await cm.DeleteCycle(cyclesEnumerable.First().Id);
        Assert.False(isdeleted);
    }

    [Fact]
    public async Task CycleModel_HasTestPlan()
    {
        var cycle = await db.Cycles.FirstOrDefaultAsync(t => t.Name == "Cycle 1");
        Assert.NotNull(cycle);
        var hasTestPlans = cm.HasTestPlans(cycle);
        Assert.True(hasTestPlans);
    }

    [Fact]
    public async Task CycleModel_HasNotTestPlan()
    {
        var cycle = await db.Cycles.FirstOrDefaultAsync(t => t.Name == "Cycle 2");
        Assert.NotNull(cycle);
        var hasTestPlans = cm.HasTestPlans(cycle);
        Assert.False(hasTestPlans);
    }

    [Fact]
    public async Task CycleModel_AddCycle()
    {
        var initialCount = await db.Cycles.CountAsync();
        var project = await pm.GetProjectById(1);

        var cycle = new Cycles
        {
            Name = "Semo Cycle",
            ProjectsId = project.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10)
        };

        await cm.AddCycle(cycle, project.Id);
        var finalCount = await db.Cycles.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task CycleModel_UpdateCycle()
    {
        var project = await pm.GetProjectById(1);

        var cycle = new Cycles
        {
            Name = "demo Cycle",
            ProjectsId = project.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(10)
        };

        await cm.AddCycle(cycle, project.Id);

        var cycles1 = await cm.GetCyclesByProjectId(project.Id);
        var firstCycle = cycles1.FirstOrDefault(x => x.Name == "demo Cycle");
        Assert.NotNull(firstCycle);
        Assert.Equal("demo Cycle", firstCycle.Name);

        cycle.Name = "demo Cycle Updated";

        await cm.UpdateCycle(cycle);

        var cycles = await cm.GetCyclesByProjectId(project.Id);
        var updatedCycle = cycles.FirstOrDefault(x => x.Name == "demo Cycle Updated");

        Assert.NotNull(updatedCycle);
        Assert.Equal("demo Cycle Updated", updatedCycle.Name);
    }
}