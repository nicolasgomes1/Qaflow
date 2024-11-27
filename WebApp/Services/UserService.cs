using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;

namespace WebApp.Services;
public class UserService(AuthenticationStateProvider authenticationStateProvider, UserManager<ApplicationUser> userManager)
{
    
    /// <summary>
    /// Retrieves the current user's information including their username and user ID.
    /// </summary>
    /// <returns>
    /// A tuple containing the username and user ID of the current user.
    /// If the user is not authenticated, both values will be null.
    /// </returns>
    public async Task<(string? UserName, string? UserId)> GetCurrentUserInfoAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated != true) return (null, null);
        var userName = user.Identity.Name;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return (userName, userId);
    }
    
    /// <summary>
    /// Retrieves the current user's username.
    /// </summary>
    /// <returns>
    /// The username of the current user, or null if the user is not authenticated.
    /// </returns>
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
    
    
 /// <summary>
 /// Get the user name from the user id
 /// </summary>
 /// <param name="id"></param>
 /// <returns></returns>
 public string GetUserNameFromUserId(string id)
 {
     var users = userManager.Users.ToList();
     return users.FirstOrDefault(x => x.Id == id)?.UserName ?? string.Empty;
 }
 
 
 /// <summary>
 /// Retrieve the list of users based on roles other than admin
 /// </summary>
 /// <returns></returns>
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


