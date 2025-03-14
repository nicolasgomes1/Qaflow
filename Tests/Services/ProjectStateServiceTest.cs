using WebApp.Services;

namespace Tests.Services;

[TestClass]
public class ProjectStateServiceTest
{
    private const int TestProject = 1;
    private readonly ProjectStateService _projectStateService = new();
    
    [TestMethod]
    public void TestProjectCanBeSet()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.AreEqual(TestProject, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public void TestProjectCanBeCleared()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.AreEqual(TestProject, _projectStateService.ProjectId);
        
        _projectStateService.ClearProjectId();
        Assert.AreEqual(0, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public void TestProjectCanBeRetrieved()
    {
        _projectStateService.SetProjectId(TestProject);
        Assert.AreEqual(TestProject, _projectStateService.ProjectId);
        
        var currentProject = _projectStateService.GetProjectIdAsync().Result;
        Assert.AreEqual(currentProject, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public async Task TestProjectCanBeRetrievedButCanError()
    {
        _projectStateService.SetProjectId(0);
        Assert.AreEqual(0, _projectStateService.ProjectId);
        
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await _projectStateService.GetProjectIdAsync());        
    }
    
}