using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;

namespace WebApp.Models;

public class CyclesModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
{
    private static readonly ILoggerFactory LoggerFactory =
        Microsoft.Extensions.Logging.LoggerFactory.Create(builder => builder.AddConsole());

    private static readonly ILogger Logger = LoggerFactory.CreateLogger(nameof(CyclesModel));


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
        Logger.LogInformation($"Cycle {cycles.Name} added to project {projectId}");
        return cycles;
    }

    public async Task<Cycles> UpdateCycle(Cycles cycles)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.Cycles.Update(cycles);
        cycles.ModifiedBy = userService.GetCurrentUserInfoAsync().Result.UserName;
        cycles.ModifiedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();
        Logger.LogInformation($"Cycle {cycles.Name} updated");
        return cycles;
    }


    public async Task<Cycles> GetCycleById(int cycleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Getting cycle {cycleId}");
        return await db.Cycles.FindAsync(cycleId) ?? throw new Exception("Cycle not found");
    }

    public async Task<List<Cycles>> GetCyclesByProjectId(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Getting cycles for project {projectId}");
        return await db.Cycles.Where(c => c.ProjectsId == projectId).ToListAsync();
    }

    public async Task<bool> DeleteCycle(int cycleId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();

        // First check if there are any test plans using this cycle
        var hasTestPlans = await db.TestPlans
            .AnyAsync(tp => tp.CycleId == cycleId);

        if (hasTestPlans)
        {
            return false; // Cannot delete due to associated test plans
        }

        var cycle = await db.Cycles
            .FirstOrDefaultAsync(c => c.Id == cycleId);

        if (cycle == null)
            throw new Exception("Cycle not found");

        db.Cycles.Remove(cycle);
        await db.SaveChangesAsync();
        Logger.LogInformation($"Cycle {cycle.Name} deleted");
        return true; // Successfully deleted
    }

    public bool HasTestPlans(Cycles cycle)
    {
        using var db = dbContextFactory.CreateDbContextAsync();
        Logger.LogInformation($"Checking if cycle {cycle.Name} has test plans");
        return db.Result.TestPlans.Any(tp => tp.CycleId == cycle.Id);
    }


    public async Task AddCyclesFromCsv(Cycles cycles, int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        cycles.ProjectsId = projectId;
        cycles.ArchivedStatus = ArchivedStatus.Active;
        db.Cycles.Add(cycles);
        await db.SaveChangesAsync(userService);
        Logger.LogInformation($"Cycle {cycles.Name} added from csv");
    }
}