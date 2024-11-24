using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

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
}


