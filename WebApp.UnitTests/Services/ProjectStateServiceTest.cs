using JetBrains.Annotations;
using WebApp.Services;

namespace WebApp.UnitTests.Services;

[TestSubject(typeof(ProjectStateService))]

public class ProjectStateServiceTest
{
    private const int TestProject = 1;
    private readonly ProjectStateService _projectStateService = new();
    
    [Fact]
    public void TestProjectCanBeSet()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.Equal(TestProject, _projectStateService.ProjectId);
    }
    
    [Fact]
    public void TestProjectCanBeCleared()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.Equal(TestProject, _projectStateService.ProjectId);
        
        _projectStateService.ClearProjectId();
        Assert.Equal(0, _projectStateService.ProjectId);
    }
    
    [Fact]
    public void TestProjectCanBeRetrieved()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.Equal(TestProject, _projectStateService.ProjectId);
        
        var currentProject = _projectStateService.GetProjectIdAsync().Result;
        Assert.Equal(currentProject, _projectStateService.ProjectId);
    }
    
    [Fact]
    public async Task TestProjectCanBeRetrievedButCanError()
    {
        _projectStateService.SetProjectId(0);
        Assert.Equal(0, _projectStateService.ProjectId);
        
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await _projectStateService.GetProjectIdAsync());
    }
    
}