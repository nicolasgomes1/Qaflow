using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class AuditTrailModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<AuditLog>> GetAuditTrailsAsync(string id)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        return await db.AuditLogs.Where(t => t.EntityId == id).ToListAsync();
    }
}