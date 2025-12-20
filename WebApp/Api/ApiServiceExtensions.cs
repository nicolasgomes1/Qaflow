using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WebApp.Api.Dto;
using WebApp.Api.Jira;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Api;

public static class ApiServiceExtensions
{
    public static void MapOwnAppApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{**catchAll}", () =>
            {
                return Results.Content(@"
        <html>
            <head>
                <title>Not Found</title>
                <style>
                    body {
                        font-family: Arial, sans-serif;
                        margin: 0;
                        padding: 20px;
                    }
                    .error-message {
                        color: red;
                        font-size: 24px;
                        font-weight: bold;
                    }
                </style>
            </head>
            <body>
                <h1 class='error-message d-flex align-items-center justify-content-center'>404 - Not Found</h1>
                <p class='error-message d-flex align-items-center justify-content-center'>The page you're looking for cannot be found.</p>
            </body>
        </html>
    ", "text/html");
            })
            .WithDisplayName("NotFoundHandler");


        app.MapGet("/api/testcases", async (FetchApiData fetchApiData) =>
        {
            var allTestCases = await fetchApiData.Api_GetListTestCases();
            return Results.Ok(allTestCases);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the List of test cases.";
            return Task.CompletedTask;
        });

        app.MapGet("/api/testcases/view/{id}", async (FetchApiData fetchApiData, int id) =>
        {
            var testCase = await fetchApiData.Api_GetTestCases(id);
            return Results.Ok(testCase);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the test case based on Id.";
            return Task.CompletedTask;
        });


        app.MapGet("/api/requirements", async (ApplicationDbContext dbContext) =>
        {
            var allRequirements = await dbContext.Requirements
                .Select(r => new RequirementsDto
                {
                    Name = r.Name,
                    Description = r.Description,
                    Priority = r.Priority.ToString(),
                    ArchivedStatus = r.ArchivedStatus,
                    ProjectId = r.ProjectsId
                }).ToListAsync();
            return Results.Ok(allRequirements);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the List of requirements.";
            return Task.CompletedTask;
        });


        app.MapGet("/api/requirements/view/{id}", async (ApplicationDbContext dbContext, int id) =>
        {
            var requirement = await dbContext.Requirements
                .Where(r => r.Id == id)
                .Select(r => new RequirementsDto
                {
                    Name = r.Name,
                    Description = r.Description,
                    Priority = r.Priority.ToString(),
                    ArchivedStatus = r.ArchivedStatus,
                    ProjectId = r.ProjectsId
                }).FirstOrDefaultAsync();
            return requirement != null ? Results.Ok(requirement) : Results.NotFound();
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the requirement by id.";
            return Task.CompletedTask;
        });

        app.MapDelete("/api/requirements/delete/{id}",
            async (ApplicationDbContext dbContext, int id) =>
            {
                var requirement = await dbContext.Requirements.FindAsync(id);
                if (requirement == null) return Results.NotFound();
                dbContext.Requirements.Remove(requirement);
                await dbContext.SaveChangesAsync();
                return Results.Ok($"Requirement {id} deleted");
            }).AddOpenApiOperationTransformer((operation, context, ct) =>
            {
                operation.Summary     = "Delete the requirement by id.";
                return Task.CompletedTask;
            })
            .RequireAuthorization(new AuthorizeAttribute
            { AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme });

        // Endpoint to create a new requirement
        app.MapPost("/api/requirements", async (ApplicationDbContext dbContext, RequirementsDto dto) =>
        {
            var newRequirement = new Requirements
            {
                Name = dto.Name,
                Description = dto.Description,
                Priority = Enum.Parse<Priority>(dto.Priority),
                ArchivedStatus = dto.ArchivedStatus,
                ProjectsId = dto.ProjectId
            };

            dbContext.Requirements.Add(newRequirement);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/requirements/{newRequirement.Id}", newRequirement);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "creates a new requirement.";
            return Task.CompletedTask;
        });

        app.MapPost("/api/projects", async (ApplicationDbContext dbContext, ProjectsDto dto) =>
        {
            var newProject = new Projects
            {
                Name = dto.Name ?? string.Empty,
                Description = dto.Description ?? string.Empty,
                ArchivedStatus = dto.ArchivedStatus
            };

            dbContext.Projects.Add(newProject);
            await dbContext.SaveChangesAsync();

            return Results.Created($"/api/projects/{newProject.Id}", newProject);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "adds new project.";
            return Task.CompletedTask;
        });

        app.MapGet("/api/projects", async (ApplicationDbContext dbContext) =>
        {
            var allProjects = await dbContext.Projects
                .Select(p => new ProjectsDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    ArchivedStatus = p.ArchivedStatus
                }).ToListAsync();
            return Results.Ok(allProjects);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the List of projects.";
            return Task.CompletedTask;
        });;

        app.MapGet("/api/projects/view/{id}", async (ApplicationDbContext dbContext, int id) =>
        {
            var project = await dbContext.Projects
                .Where(p => p.Id == id)
                .Select(p => new ProjectsDto
                {
                    Name = p.Name,
                    Description = p.Description,
                    ArchivedStatus = p.ArchivedStatus
                }).FirstOrDefaultAsync();
            return project != null ? Results.Ok(project) : Results.NotFound("No project found");
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "view project by id.";
            return Task.CompletedTask;
        });

        app.MapDelete("/api/projects/delete/{id}", async (ApplicationDbContext dbContext, int id) =>
        {
            var project = await dbContext.Projects.FindAsync(id);
            try
            {
                if (project is null) return Results.NotFound($"Project {id} not found");
                dbContext.Projects.Remove(project);
                await dbContext.SaveChangesAsync();
                return Results.Ok($"Project {id} deleted");
            }
            catch (DbUpdateException ex)
            {
                return Results.Conflict($"Cannot delete project {id} due to related data. Error: {ex.Message}");
            }
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "delete project by id.";
            return Task.CompletedTask;
        });


        app.MapGet("/api/bugs", async (ApplicationDbContext dbContext) =>
        {
            var allBugs = await dbContext.Bugs
                .Select(b => new BugsDto
                {
                    Name = b.Name,
                    Description = b.Description,
                    Priority = b.Priority.ToString(),
                    Severity = b.Severity.ToString(),
                    BugStatus = b.BugStatus.ToString(),
                    ArchivedStatus = b.ArchivedStatus.ToString(),
                    ProjectId = b.ProjectsId
                }).ToListAsync();
            return Results.Ok(allBugs);
        }).AddOpenApiOperationTransformer((operation, context, ct) =>
        {
            operation.Summary     = "Gets the List of bugs.";
            return Task.CompletedTask;
        });
    }


    public static void MapJiraApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/jiratickets", async (JiraService jiraService) =>
        {
            try
            {
                const string projectId = "MFLP";
                var issues = await jiraService.GetProjectIssuesAsync(projectId);
                return Results.Ok(issues);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error fetching Jira issues: {ex.Message}");
            }
        });

        app.MapGet("/api/jiraticketsdb", async (JiraServiceFromDb jiraService) =>
        {
            try
            {
                const string projectId = "MFLP";
                const string uniqueKey = "Jira-01";
                var issues = await jiraService.GetProjectIssuesFromDbAsync(uniqueKey, projectId);
                return Results.Ok(issues);
            }
            catch (Exception ex)
            {
                return Results.Problem($"Error fetching Jira issues: {ex.Message}");
            }
        });
    }
}