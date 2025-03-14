using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApp.Data;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.UnitTests.Models;

[TestClass]
[TestSubject(typeof(ProjectModel))]
public class ProjectModelTest
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly ApplicationDbContext _dbContext;
    
    
    private readonly UserService _userService;
    private readonly ProjectStateService _projectStateService; 
    private readonly ProjectModel _projectModel;
    public ProjectModelTest(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
        _dbContext = _dbContextFactory.CreateDbContext();
    }

    [TestMethod]
    public async Task TestCreateProject()
    {
        Projects projects = new()
        {
            Name = "Project 1",
            Description = "Description 1"
        };
        
        await _projectModel.AddProject(projects);

        
    
        var createdProject = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Name == "Project 1");
    
        Assert.IsNotNull(createdProject);
        Assert.AreEqual("Project 1", createdProject.Name);
        Assert.AreEqual("Description 1", createdProject.Description);

        await _projectModel.RemoveProject(projects.Id);
    }
}

