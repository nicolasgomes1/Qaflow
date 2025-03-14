using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Controllers;

namespace WebApp.UnitTests.Controllers;

[TestClass]
[TestSubject(typeof(CultureController))]
public class CultureControllerTest
{

    [TestMethod]
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
        Assert.IsNotNull(result);
        Assert.AreEqual(redirectUri, result.Url);
        Assert.AreEqual(redirectUri, result.Url);
        var cookieHeader = httpContext.Response.Headers["Set-Cookie"].ToString();
        Assert.IsTrue(cookieHeader.Contains(CookieRequestCultureProvider.DefaultCookieName));
        var expectedCookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
        Assert.AreEqual(expectedCookieValue, GetCookieValue(cookieHeader, CookieRequestCultureProvider.DefaultCookieName));
    }
    
    private static string GetCookieValue(string cookieHeader, string cookieName)
    {
        var cookies = cookieHeader.Split(';');
        foreach (var cookie in cookies)
        {
            var parts = cookie.Split('=');
            if (parts[0].Trim() == cookieName)
            {
                return System.Net.WebUtility.UrlDecode(parts[1].Trim());
            }
        }
        return string.Empty;
    }
}