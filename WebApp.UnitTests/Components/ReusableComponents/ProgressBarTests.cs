using Bunit;
using WebApp.Components.ReusableComponents;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Components.ReusableComponents;

public class ProgressBarTests : BaseComponentTest
{
    public ProgressBarTests(TestFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void CanRenderTheProgressBar()
    {
        var cut = Render<ProgressBar>();

        Assert.NotNull(cut.Find("[data-testid='progress_bar']"));
    }

    [Fact]
    public void CanRenderTheProgressBarWithDIfferentParameter()
    {
        var cut = Render<ProgressBar>(p => p.Add(r => r.Template, "Fuck Off"));

        Assert.NotNull(cut.Find("[data-testid='progress_bar']"));
    }
}