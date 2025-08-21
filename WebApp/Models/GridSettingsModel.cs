using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Services;

namespace WebApp.Models;

public class GridSettingsModel(IDbContextFactory<ApplicationDbContext> dbContextFactory, UserService userService)
{
    public async Task<GridSettings> GetCUrrentGridUserSettings(string gridName)
    {
        await using var db = await dbContextFactory.CreateDbContextAsync();
        var user = userService.GetCurrentUserInfoAsync().Result.UserName;
        if (user == null) throw new Exception("User not found");

        var gridSetting = await db.GridSettings
            .Where(x => x.UserName == user && x.GridName == gridName)
            .FirstOrDefaultAsync();

        if (gridSetting != null) return gridSetting;
        gridSetting = new GridSettings
        {
            UserName = user,
            GridName = gridName
        };
        db.GridSettings.Add(gridSetting);
        await db.SaveChangesAsync();

        return gridSetting;
    }


    public async Task<IEnumerable<GridSettings>> LoadGridSettings()
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var user = userService.GetCurrentUserInfoAsync().Result.UserName;

        if (string.IsNullOrEmpty(user))
            throw new Exception("User not found");

        return await dbContext.GridSettings
            .Where(gs => gs.UserName == user)
            .ToListAsync();
    }

    public async Task DeleteGridSettings(GridSettings gridSettings)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.GridSettings.Remove(gridSettings);
        await dbContext.SaveChangesAsync();
    }

    public async Task AddGridSettings(GridSettings gridSettings)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.GridSettings.Add(gridSettings);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateGridSettings(GridSettings gridSettings)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.GridSettings.Update(gridSettings);
        await dbContext.SaveChangesAsync();
    }
}