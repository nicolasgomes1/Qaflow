using WebApp.UnitTests.DIContainers;
using WebApp.UnitTests.Models;

namespace WebApp.UnitTests.BaseTest;

public class TestBase : IClassFixture<TestFixture>
{
    protected readonly IServiceProvider ServiceProvider;

    public TestBase(TestFixture fixture)
    {
        ServiceProvider = fixture.ServiceProvider;
    }

    // You can add common setup logic here
}
