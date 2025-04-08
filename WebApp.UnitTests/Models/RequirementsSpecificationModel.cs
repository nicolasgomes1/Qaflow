using JetBrains.Annotations;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebApp.Data;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(RequirementsSpecificationModel))]
public class RequirementsSpecificationModelTest : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly RequirementsSpecificationModel rm;

    public RequirementsSpecificationModelTest(TestFixture fixture)
    {
        // Resolve services via ServiceProvider
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        rm = fixture.ServiceProvider.GetRequiredService<RequirementsSpecificationModel>();
    }

    [Fact]
    public async Task RequirementsSpecificationModel_AddRequirementsSpecification()
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");

        var initialCount = await db.RequirementsSpecification.CountAsync();
        if (project == null) throw new Exception("Project not found");
        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "sample",
            Description = "Description"
        };

        await rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        var finalCount = await db.RequirementsSpecification.CountAsync();
        Assert.Equal(initialCount + 1, finalCount);
    }

    [Fact]
    public async Task RequirementsSpecificationModel_UpdateRequirementsSpecification()
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");
        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "sample",
            Description = "Description"
        };

        await rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);

        var currentRequirementsSpecification =
            await db.RequirementsSpecification.FirstOrDefaultAsync(x => x.Name == "sample");
        if (currentRequirementsSpecification == null) throw new Exception("Requirements Specification not found");
        currentRequirementsSpecification.Name = "updated";
        currentRequirementsSpecification.Description = "updated description";

        await rm.UpdateRequirementsSpecificationAsync(currentRequirementsSpecification);
        var updatedRequirementsSpecification =
            await db.RequirementsSpecification.FirstOrDefaultAsync(x => x.Name == "updated");
        if (updatedRequirementsSpecification == null) throw new Exception("Requirements Specification not found");
        Assert.Equal("updated", updatedRequirementsSpecification.Name);
        Assert.Equal("updated description", updatedRequirementsSpecification.Description);
    }

    [Fact]
    public async Task RequirementsSpecificationModel_GetRequirementsSpecifications()
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");

        for (var i = 0; i < 3; i++)
        {
            var newRequirementsSpecification = new RequirementsSpecification
            {
                Name = $"sample{i}",
                Description = $"Description{i}"
            };
            await rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        }

        var requirementsSpecifications = await rm.GetRequirementsSpecificationListAsync(project.Id);

        var filteredRequirementsSpecifications =
            requirementsSpecifications.Where(x => x.Name == "sample1" || x.Name == "sample2").ToList();

        Assert.Equal(2, filteredRequirementsSpecifications.Count);
    }


    [Fact]
    public async Task DeleteRequirementsSpecificationFromMethod()
    {
        var project = await db.Projects.FirstOrDefaultAsync(x => x.Name == "Demo Project With Data");
        if (project == null) throw new Exception("Project not found");


        var newRequirementsSpecification = new RequirementsSpecification
        {
            Name = "found",
            Description = "Description"
        };
        var added = await rm.AddRequirementsSpecification(newRequirementsSpecification, project.Id);
        var searched = await rm.GetRequirementsSpecificationByIdAsync(added.Id);

        await rm.DeleteRequirementsSpecification(searched.Id);

        Assert.Equal(0, await db.RequirementsSpecification.CountAsync(x => x.Name == "found"));
    }
}