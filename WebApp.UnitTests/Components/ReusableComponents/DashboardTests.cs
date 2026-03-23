using Bunit;
using WebApp.Components.ReusableComponents;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components.ReusableComponents;

public class DashboardTests : BaseComponentTest
{
    public DashboardTests(TestFixture fixture) : base(fixture)
    {
    }


    [Fact]
    public void HelloWorldComponentRendersCorrectly()
    {
        // Act
        var cut = Render<Dashboard>(parameters => parameters
            .Add(p => p.ProjectId, 1)); // Dashboard requires ProjectId parameter

        // Assert
        // Update the assertion to match what Dashboard actually renders
        Assert.NotNull(cut.Find("[data-testid='d_requirementsSpecification']"));
        Assert.NotNull(cut.Find("[data-testid='d_testcases']"));
        Assert.NotNull(cut.Find("[data-testid='d_bugs']"));
    }
}