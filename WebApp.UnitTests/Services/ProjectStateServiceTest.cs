using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Services;

namespace WebApp.UnitTests.Services;

[TestClass]
[TestSubject(typeof(ProjectStateService))]
public class ProjectStateServiceTest
{
    private readonly int _testProject = 1;
    private ProjectStateService _projectStateService = new ProjectStateService();
    
    [TestMethod]
    public void TestProjectCanBeSet()
    {
        _projectStateService.SetProjectId(_testProject);
        Assert.AreEqual(_testProject, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public void TestProjectCanBeCleared()
    {
        _projectStateService.SetProjectId(_testProject);
        Assert.AreEqual(_testProject, _projectStateService.ProjectId);
        
        _projectStateService.ClearProjectId();
        Assert.AreEqual(0, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public void TestProjectCanBeRetrieved()
    {
        _projectStateService.SetProjectId(_testProject);
        Assert.AreEqual(_testProject, _projectStateService.ProjectId);
        
        var currentProject = _projectStateService.GetProjectIdAsync().Result;
        Assert.AreEqual(currentProject, _projectStateService.ProjectId);
    }
    
    [TestMethod]
    public void TestProjectCanBeRetrievedButCanError()
    {
        _projectStateService.SetProjectId(0);
        Assert.AreEqual(0, _projectStateService.ProjectId);
        
        Assert.ThrowsExceptionAsync<InvalidOperationException>(async () => await _projectStateService.GetProjectIdAsync());
        
    }
    
}