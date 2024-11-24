using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using WebApp.Api.Jira;
using WebApp.Components;
using WebApp.Components.Account;
using WebApp.Data;
using WebApp.ServiceDefaults;
using WebApp.Services.TestData;
using WebApp.SetUp;
using Scalar.AspNetCore;
using WebApp.Api;


var builder = WebApplication.CreateBuilder(args);

builder.AddSqlServerDbContext<ApplicationDbContext>("qa");

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region AddIdentity
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme;
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddIdentityCookies();
#endregion





builder.Services.AddDbContextFactory<ApplicationDbContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

builder.Services.AddRadzenComponents();

// Add seeding services to the container.
builder.Services.AddSeedingServices();

//add additional services
builder.Services.AddAppServices();

// Add models to the container.
builder.Services.AddModels();

builder.Services.AddLocalization();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.Configure<JiraApiOptions>(builder.Configuration.GetSection("JiraApi"));
// Register JiraService with HttpClient for dependency injection
builder.Services.AddHttpClient<JiraService>();
builder.Services.AddHttpClient<JiraServiceFromDb>();


var app = builder.Build();

var supportedCultures = new[] { "en-US", "fr-BE", "nl-BE" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.MapDefaultEndpoints();

await app.ConfigureDatabaseAsync();


app.MapControllers();

#region Api
app.MapOwnAppApiEndpoints();
app.MapJiraApiEndpoints();
#endregion


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.UseMigrationsEndPoint();
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
