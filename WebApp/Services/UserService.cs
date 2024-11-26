using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using WebApp.Data;

namespace WebApp.Services;
public class UserService(AuthenticationStateProvider authenticationStateProvider)
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
}


