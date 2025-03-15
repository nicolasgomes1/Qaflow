using WebApp.Models;
using WebApp.UnitTests.BaseTest;

namespace WebApp.UnitTests.Models;

    public class MyServiceTests : TestBase
    {
        public MyServiceTests(TestFixture fixture) : base(fixture) { }

        [Fact]
        public void MyService_ReturnsExpectedData()
        {
            // Resolve the service using the ServiceProvider
            var myService = ServiceProvider.GetRequiredService<ProjectModel>();


            for (int i = 0; i < 3; i++)
            {
                var newProject = new Data.Projects
                {
                    Name = $"sample{i}",
                    Description = $"Description{i}"
                };

                myService.AddProject(newProject);
            }
            
            
    
            var result = myService.GetProjects();

            Assert.Equal(3, result.Result.Count);
        }
    }

