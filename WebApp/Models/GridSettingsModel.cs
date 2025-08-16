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
}