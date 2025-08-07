using Microsoft.AspNetCore.Components.Authorization;

namespace WebApp.Services;

public class UiUserVisibility
{
    public UiUserVisibility(AuthenticationStateProvider authenticationStateProvider)
    {
        AuthenticationStateProvider = authenticationStateProvider;
    }

    public AuthenticationStateProvider AuthenticationStateProvider { get; set; }


    /// <summary>
    /// Only admin and Manager are authorized
    /// </summary>
    /// <returns></returns>
    public bool IsAuthorized()
    {
        var user = AuthenticationStateProvider.GetAuthenticationStateAsync().Result.User;
        return user.IsInRole("Admin") || user.IsInRole("Manager");
    }
}