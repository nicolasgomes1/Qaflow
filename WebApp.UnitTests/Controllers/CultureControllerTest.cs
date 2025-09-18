using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Components.Layout;
using WebApp.Controllers;

namespace WebApp.UnitTests.Controllers;

[TestSubject(typeof(CultureSelector))]
public class CultureControllerTest
{
    [Fact]
    public void TestCultureIsSetCorrectly()
    {
        // Arrange
        var controller = new CultureController();
        var culture = "en-US";
        var redirectUri = "/home";
        var httpContext = new DefaultHttpContext();
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        // Act
        var result = controller.Set(culture, redirectUri) as LocalRedirectResult;

        // Assert
        Assert.NotNull(result);
        Assert.Equal(redirectUri, result.Url);
        Assert.Equal(redirectUri, result.Url);
        var cookieHeader = httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.Contains(CookieRequestCultureProvider.DefaultCookieName, cookieHeader);
        var expectedCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
        Assert.Equal(expectedCookieValue, GetCookieValue(cookieHeader, CookieRequestCultureProvider.DefaultCookieName));
    }

    private static string GetCookieValue(string cookieHeader, string cookieName)
    {
        var cookies = cookieHeader.Split(';');
        foreach (var cookie in cookies)
        {
            var parts = cookie.Split('=');
            if (parts[0].Trim() == cookieName) return WebUtility.UrlDecode(parts[1].Trim());
        }

        return string.Empty;
    }
}