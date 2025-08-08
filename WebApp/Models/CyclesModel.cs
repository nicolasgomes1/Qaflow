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

    public async Task<Cycles> UpdateCycle(Cycles cycles)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Cycles.Update(cycles);
        cycles.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        cycles.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        return cycles;
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

    public async Task<bool> DeleteCycle(int cycleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        // Check if cycle exists and include related test plans
        var cycle = await db.Cycles
            .Include(c => c.TestPlans)
            .FirstOrDefaultAsync(c => c.Id == cycleId);

        if (cycle == null) throw new Exception("Cycle not found");

        // Check if there are any test plans associated with this cycle
        if (cycle.TestPlans.Any())
        {
            return true; // Cannot delete cycle because it has associated test plans
        }

        db.Cycles.Remove(cycle);
        await db.SaveChangesAsync();
        return false; // Successfully deleted cycle
    }
}