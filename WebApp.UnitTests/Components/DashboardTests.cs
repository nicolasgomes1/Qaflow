using Bunit;
using WebApp.Api.Jira;
using WebApp.Components.ReusableComponents;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components;
using Xunit;

public class DashboardTests : TestContext, IClassFixture<TestFixture>
{
    private readonly TestCasesReporting _reporting;
    
    public DashboardTests(TestFixture fixture)
    {
        _reporting = fixture.ServiceProvider.GetRequiredService<TestCasesReporting>();
        
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

        // Add any other services that Dashboard uses through @inject directives
    }
    
    [Fact]
    public void HelloWorldComponentRendersCorrectly()
    {
        // Act
        var cut = RenderComponent<Dashboard>(parameters => parameters
            .Add(p => p.ProjectId, 1)); // Dashboard requires ProjectId parameter

        // Assert
        // Update the assertion to match what Dashboard actually renders
        Assert.NotNull(cut.Find("[data-testid='d_requirementsSpecification']"));
        Assert.NotNull(cut.Find("[data-testid='d_testcases']"));
        Assert.NotNull(cut.Find("[data-testid='d_bugs']"));
    }
}