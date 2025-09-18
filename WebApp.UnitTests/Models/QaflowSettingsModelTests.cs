using JetBrains.Annotations;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Models;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Models;

[TestSubject(typeof(QAflowSettings))]
public class QaflowSettingsModelTests : IClassFixture<TestFixture>
{
    private readonly ApplicationDbContext db;
    private readonly QAflowSettingsModel qaflowSettingsModel;

    public QaflowSettingsModelTests(TestFixture fixture)
    {
        db = fixture.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        qaflowSettingsModel = fixture.ServiceProvider.GetRequiredService<QAflowSettingsModel>();
    }

    [Fact]
    public async Task QaflowSettingsModel_GetSettings()
    {
        var settings = await qaflowSettingsModel.GetApplicationSettingsAsync();
        Assert.NotNull(settings);
        Assert.Contains(settings, x => x.QAflowOptionsSettings == QAflowOptionsSettings.ExternalIntegrations);
        Assert.Contains(settings, x => x.QAflowOptionsSettings == QAflowOptionsSettings.OwnIntegrations);
        Assert.Equal(2, settings.Count);
    }

    [Fact]
    public async Task QaflowSettingsModel_UpdateSettings()
    {
        var settings = await qaflowSettingsModel.GetApplicationSettingsAsync();
        Assert.NotNull(settings);

        var selectedSetting =
            settings.FirstOrDefault(x => x.QAflowOptionsSettings == QAflowOptionsSettings.ExternalIntegrations);
        Assert.NotNull(selectedSetting);
        Assert.False(selectedSetting.IsIntegrationEnabled);

        // Toggle the IsIntegrationEnabled property
        selectedSetting.IsIntegrationEnabled = true;
        var updated = await qaflowSettingsModel.UpdateApplicationSettingsAsync(selectedSetting);

        var newSettings = await qaflowSettingsModel.GetApplicationSettingsAsync();
        var updatedfromdb =
            newSettings.FirstOrDefault(x => x.QAflowOptionsSettings == QAflowOptionsSettings.ExternalIntegrations);
        Assert.True(updatedfromdb is { IsIntegrationEnabled: true });
    }
}