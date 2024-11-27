using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Services;
public class UserService(AuthenticationStateProvider authenticationStateProvider, UserManager<ApplicationUser> userManager)
{
    public async Task<(string? UserName, string? UserId)> GetCurrentUserInfoAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated != true) return (null, null);
        var userName = user.Identity.Name;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return (userName, userId);
    }
    
    public async Task<string?> GetCurrentUserNameAsync()
    {
        var userInfo = await GetCurrentUserInfoAsync();
        return userInfo.UserName;
    }
    
    /// <summary>
    /// Returns the name of the user with the given id. To be used in the dropdowns
    /// </summary>
    /// <param name="userId"> The selected User in the dropdown with its guid</param>
    /// <param name="userList">The List of Users </param>
    /// <returns></returns>
    public static string GetUserName(string userId, List<ApplicationUser> userList)
    {
        var user = userList.FirstOrDefault(user => user.Id == userId);
        return user?.UserName ?? "Unassigned";
    }
    
    
    
    public string GetUserNameFromUserId(string id)
    {
        return userManager.Users.FirstOrDefault(x => x.Id == id)?.UserName ?? string.Empty;
    }
    
    public async Task<List<ApplicationUser>> GetUsersList()
    {
        // Get all users
        var users = await userManager.Users.ToListAsync();

        // Filter out users with the "Admin" role
        var nonAdminUsers = new List<ApplicationUser>();
        foreach (var user in users)
        {
            if (!await userManager.IsInRoleAsync(user, "Admin"))
            {
                nonAdminUsers.Add(user);
            }
        }

        return nonAdminUsers;
    }
    
}


