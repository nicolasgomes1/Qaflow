using Bunit;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;
using WebApp.Api.Jira;
using WebApp.Components.ReusableComponents;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components;
using Xunit;

public class DashboardTests : TestContext, IClassFixture<TestFixture>
{
    
    public DashboardTests(TestFixture fixture)
    {
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectModel>());
        // Register all services from the fixture with the BUnit TestContext
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesReporting>());
        // Add other services that Dashboard component might need
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestExecutionModelv2>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ProjectState>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsSpecificationModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesJiraModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<GenerateJwtToken>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<BugsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<BugsFilesModel>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<JiraServiceFromDb>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<JiraService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<IntegrationsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestPlansFilesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ReportsModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<RequirementsFilesModel>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<Radzen.NotificationService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<Radzen.ContextMenuService>());

        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<FormNotificationService>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ApplicationUser>());
        Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<TestCasesFilesModel>());

        Services.AddSingleton<UserManager<ApplicationUser>, TestUserManager>();

       // Services.AddSingleton(fixture.ServiceProvider.GetRequiredService<ResourceManagerStringLocalizerFactory>());

        // Add localization services - DON'T try to get ResourceManagerStringLocalizerFactory
        Services.AddSingleton<IEmailSender<ApplicationUser>, TestEmailSender>();
        Services.AddSingleton<RoleManager<IdentityRole>, TestRoleManager>();

        // Create a simple dummy localizer for the Resources type
        Services.AddLocalization();


    }
    
    [Fact]
    public void HelloWorldComponentRendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Dashboard>(parameters => parameters
            .Add(p => p.ProjectId, 1)); // Dashboard requires ProjectId parameter
        var localizer = Services.GetService<IStringLocalizer<SharedStrings>>();

        // Assert
        // Update the assertion to match what Dashboard actually renders
        Assert.NotNull(cut.Find("[data-testid='d_requirementsSpecification']"));
        Assert.NotNull(cut.Find("[data-testid='d_testcases']"));
        Assert.NotNull(cut.Find("[data-testid='d_bugs']"));
    }
}

public class SharedStrings
{
    // This class is just a marker for the resource files
}

// Test implementation of IEmailSender<ApplicationUser> for testing
public class TestEmailSender : IEmailSender<ApplicationUser>
{
    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        // Do nothing for tests
        return Task.CompletedTask;
    }
}

// Test implementation of RoleManager<IdentityRole> for testing
public class TestRoleManager : RoleManager<IdentityRole>
{
    public TestRoleManager() : base(
        new TestRoleStore(),
        null,
        null,
        null,
        null)
    {
    }
}

// Test implementation of IRoleStore<IdentityRole> for testing
public class TestRoleStore : IRoleStore<IdentityRole>
{
    public void Dispose() { }

    public Task<IdentityResult> CreateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<string> GetRoleIdAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Id);
    }

    public Task<string> GetRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.Name);
    }

    public Task SetRoleNameAsync(IdentityRole role, string roleName, CancellationToken cancellationToken)
    {
        role.Name = roleName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedRoleNameAsync(IdentityRole role, CancellationToken cancellationToken)
    {
        return Task.FromResult(role.NormalizedName);
    }

    public Task SetNormalizedRoleNameAsync(IdentityRole role, string normalizedName, CancellationToken cancellationToken)
    {
        role.NormalizedName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
    {
        return Task.FromResult<IdentityRole>(null);
    }

    public Task<IdentityRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
    {
        return Task.FromResult<IdentityRole>(null);
    }
}

// Test implementation of UserManager<ApplicationUser> for testing
public class TestUserManager : UserManager<ApplicationUser>
{
    public TestUserManager() : base(
        new TestUserStore(),
        null,
        null,
        null,
        null,
        null,
        null,
        null,
        null)
    {
    }
}

// Test implementation of IUserStore<ApplicationUser> for testing
public class TestUserStore : IUserStore<ApplicationUser>
{
    public void Dispose() { }

    public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.Id);
    }

    public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.UserName);
    }

    public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
    {
        user.UserName = userName;
        return Task.CompletedTask;
    }

    public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(user.NormalizedUserName);
    }

    public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
    {
        user.NormalizedUserName = normalizedName;
        return Task.CompletedTask;
    }

    public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
    {
        return Task.FromResult(IdentityResult.Success);
    }

    public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
    {
        return Task.FromResult<ApplicationUser>(null);
    }

    public Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
    {
        return Task.FromResult<ApplicationUser>(null);
    }
}





