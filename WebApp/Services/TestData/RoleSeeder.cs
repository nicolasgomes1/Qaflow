using Microsoft.AspNetCore.Identity;
using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Services.TestData;

public class RoleSeeder(IServiceProvider serviceProvider, ILogger<RoleSeeder> logger)
    : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Define roles you want to seed
        string[] roleNames = [nameof(UserRoles.Admin), nameof(UserRoles.User), nameof(UserRoles.Manager)];
        foreach (var roleName in roleNames)
        {
            if (await roleManager.RoleExistsAsync(roleName)) continue;
            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            if (!roleResult.Succeeded)
                logger.LogError("Error creating role {RoleName}: {Errors}", roleName,
                    string.Join(", ", roleResult.Errors.Select(e => e.Description)));
        }

        // Create users and assign them to roles
        await CreateUserAndAssignRole(userManager, "admin@example.com", "Admin123!", nameof(UserRoles.Admin), true);
        await CreateUserAndAssignRole(userManager, "user@example.com", "User123!", nameof(UserRoles.User), true);
        await CreateUserAndAssignRole(userManager, "manager@example.com", "Manager123!", nameof(UserRoles.Manager),
            true);
    }

    private async Task CreateUserAndAssignRole(UserManager<ApplicationUser> userManager, string email, string password,
        string role, bool emailConfirmed)
    {
        var user = await userManager.FindByEmailAsync(email);

        if (user == null)
        {
            user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = emailConfirmed };
            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
            {
                logger.LogError("Error creating user {email}: {Errors}", email,
                    string.Join(", ", createResult.Errors.Select(e => e.Description)));
                return;
            }
        }

        // Ensure the user is added to the role
        if (!await userManager.IsInRoleAsync(user, role))
        {
            var addToRoleResult = await userManager.AddToRoleAsync(user, role);
            if (!addToRoleResult.Succeeded)
                logger.LogError("Error assigning role {role}: {Errors}", role,
                    string.Join(", ", addToRoleResult.Errors.Select(e => e.Description)));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}