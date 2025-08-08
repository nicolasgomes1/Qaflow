using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class CyclesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
{
    public async Task ArchiveExpiredCyclesAsync()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();


        var now = DateTime.UtcNow;
        var expiredCycles = await db.Cycles
            .Where(c => c.EndDate < now && c.ArchivedStatus != ArchivedStatus.Archived)
            .ToListAsync();

        foreach (var cycle in expiredCycles)
        {
            cycle.ArchivedStatus = ArchivedStatus.Archived;
        }

        await db.SaveChangesAsync();
    }

    public async Task<Cycles> AddCycle(Cycles cycles, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Cycles.Add(cycles);
        cycles.CreatedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        cycles.CreatedAt = DateTime.UtcNow;
        cycles.ProjectsId = projectId;
        await db.SaveChangesAsync();
        return cycles;
    }

    public async Task<Cycles> UpdateCycle(Cycles cycles, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Cycles.Update(cycles);
        cycles.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        cycles.ModifiedAt = DateTime.UtcNow;
        cycles.ProjectsId = projectId;
        return cycles;
    }

    public async Task DeleteCycle(int cycleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var cycle = await db.Cycles.FindAsync(cycleId);
        if (cycle == null) throw new Exception("Cycle not found");
        db.Cycles.Remove(cycle);
        await db.SaveChangesAsync();
    }


    public async Task<Cycles> GetCycleById(int cycleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Cycles.FindAsync(cycleId) ?? throw new Exception("Cycle not found");
    }

    public async Task<List<Cycles>> GetCyclesByProjectId(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.Cycles.Where(c => c.ProjectsId == projectId).ToListAsync();
    }
}