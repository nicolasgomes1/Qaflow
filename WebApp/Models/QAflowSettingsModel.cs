using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Models;

public class QAflowSettingsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory)
{
    public async Task<List<QAflowSettings>> GetApplicationSettingsAsync(int projectId)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var settings = await db.QAflowSettings.Where(p => p.ProjectsId == projectId).ToListAsync();
        return settings;
    }

    public async Task<QAflowSettings> UpdateApplicationSettingsAsync(QAflowSettings settings)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        db.QAflowSettings.Update(settings);
        await db.SaveChangesAsync();
        return settings;
    }
}