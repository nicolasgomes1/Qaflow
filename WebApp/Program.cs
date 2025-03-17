using System.Text;
using MartinCostello.OpenApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

#if POSTGRES
    builder.AddNpgsqlDbContext<ApplicationDbContext>("postgresdb");
#elif SQLSERVER
    builder.AddSqlServerDbContext<ApplicationDbContext>("qa");
#endif

// Add authentication
    var key = builder.Configuration["Jwt:Key"]; // Store securely!
    var issuer = builder.Configuration["Jwt:Issuer"];
    var audience = builder.Configuration["Jwt:Audience"];


builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

#region AddIdentity
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

// builder.Services.AddAuthentication(options =>
//     {
//         options.DefaultScheme = IdentityConstants.ApplicationScheme;
//         options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//     })
//     .AddJwtBearer(options =>
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateIssuer = true,
//             ValidateAudience = true,
//             ValidateLifetime = true,
//             ValidateIssuerSigningKey = true,
//             ValidIssuer = issuer,
//             ValidAudience = audience,
//             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
//
//         }
//         )
//     .AddIdentityCookies();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = IdentityConstants.ApplicationScheme; // Default to Identity cookies for UI
        options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => // JWT Bearer Authentication for API requests
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key!))
        };

        // Prevent redirect to login page when an API request is unauthorized
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                return context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
            }
        };
    })
    .AddIdentityCookies(); // Identity cookies for Blazor UI authentication


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

builder.Services.AddOpenApiExtensions(options =>
{
    options.AddServerUrls = true;
});

builder.Services.Configure<JiraApiOptions>(builder.Configuration.GetSection("JiraApi"));
// Register JiraService with HttpClient for dependency injection
builder.Services.AddHttpClient<JiraService>();
builder.Services.AddHttpClient<JiraServiceFromDb>();

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins("https://localhost:7089") // Replace with your Blazor app URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICleanUpPlaywrightTestsData, CleanUpPlaywrightTestsData>();

var app = builder.Build();

// After building the app:
using (var scope = app.Services.CreateScope())
{
    var cleanupService = scope.ServiceProvider.GetRequiredService<ICleanUpPlaywrightTestsData>();
    await cleanupService.DeleteAllPlaywrightProjectData();
}

var supportedCultures = new[] { "en-US", "fr-BE", "nl-BE" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);
app.UseRequestLocalization(localizationOptions);

app.MapDefaultEndpoints();

await app.ConfigureDatabaseAsync();
// Enable CORS
app.UseCors("AllowBlazorClient");

app.MapControllers();

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

#region Api
app.MapOwnAppApiEndpoints();
app.MapJiraApiEndpoints();
#endregion

app.UseHttpsRedirection();

app.UseCors();
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
