using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(RequirementsSpecificationModel))]
public class RequirementsSpecificationModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext _db;
    private readonly RequirementsSpecificationModel _rm;

    public RequirementsSpecificationModelTests(TestFixture fixture)
    {
        // Resolve services via ServiceProvider
        _db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        _rm = fixture.ServiceProvider.GetRequiredService<RequirementsSpecificationModel>();
    }

    [Fact]
    public async Task RequirementsSpecificationModel_AddRequirementsSpecification()
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");

        var initialCount = await _db.RequirementsSpecification.CountAsync();
        if (project == null) throw new Exception("Project not found");
        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "sample",
            Description = "Description"
        };

        await _rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        var finalCount = await _db.RequirementsSpecification.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task RequirementsSpecificationModel_UpdateRequirementsSpecification()
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "sample",
            Description = "Description"
        };

        await _rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);

        var currentRequirementsSpecification =
            await _db.RequirementsSpecification.FirstOrDefaultAsync(x => x.Name == "sample");
        if (currentRequirementsSpecification == null) throw new Exception("Requirements Specification not found");
        currentRequirementsSpecification.Name = "updated";
        currentRequirementsSpecification.Description = "updated description";

        await _rm.UpdateRequirementsSpecificationAsync(currentRequirementsSpecification);
        var updatedRequirementsSpecification =
            await _db.RequirementsSpecification.FirstOrDefaultAsync(x => x.Name == "updated");
        if (updatedRequirementsSpecification == null) throw new Exception("Requirements Specification not found");
        Assert.Equal("updated", updatedRequirementsSpecification.Name);
        Assert.Equal("updated description", updatedRequirementsSpecification.Description);
    }

    [Fact]
    public async Task RequirementsSpecificationModel_GetRequirementsSpecifications()
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");

        for (var i = 0; i < 3; i++)
        {
            var newRequirementsSpecification = new RequirementsSpecification
            {
                Name = $"sample{i}",
                Description = $"Description{i}"
            };
            await _rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        }

        var requirementsSpecifications = await _rm.GetRequirementsSpecificationListAsync(project.Id);

        var filteredRequirementsSpecifications =
            requirementsSpecifications.Where(x => x.Name == "sample1" || x.Name == "sample2").ToList();

        Assert.Equal(2, filteredRequirementsSpecifications.Count);
    }


    [Fact]
    public async Task DeleteRequirementsSpecificationFromMethod()
    {
        var project = await _db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");


        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "found",
            Description = "Description"
        };
        var added = await _rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        var searched = await _rm.GetRequirementsSpecificationByIdAsync(added.Id);

        await _rm.DeleteRequirementsSpecification(searched.Id);

        Assert.Equal(0, await _db.RequirementsSpecification.CountAsync(x => x.Name == "found"));
    }
}