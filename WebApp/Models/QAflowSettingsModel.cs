using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class QAflowSettingsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<QAflowSettings>> GetApplicationSettingsAsync()
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var settings = await db.QAflowSettings.ToListAsync();
        return settings;
    }
}