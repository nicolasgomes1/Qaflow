using Radzen;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Services;

public class ManageCsvUploadTest : BaseComponentTest
{
    private readonly ManageCsvUpload _service;
    private readonly FormNotificationService _formNotificationService;
    private readonly ProjectModel _projectModel;
    private readonly RequirementsModel _requirementsModel;
    private readonly DialogService _dialogService;
    private readonly RequirementsSpecificationModel _requirementsSpecificationModel;
    private readonly TestCasesModel _testCasesModel;
    private readonly CyclesModel _cyclesModel;
    private readonly TestPlansModel _testPlansModel;
    
    public ManageCsvUploadTest(TestFixture fixture) : base(fixture)
    {
        _service = fixture.ServiceProvider.GetRequiredService<ManageCsvUpload>();
        _formNotificationService = fixture.ServiceProvider.GetRequiredService<FormNotificationService>();
        _projectModel = fixture.ServiceProvider.GetRequiredService<ProjectModel>();
        _requirementsModel = fixture.ServiceProvider.GetRequiredService<RequirementsModel>();
        _dialogService = fixture.ServiceProvider.GetRequiredService<DialogService>();
        _requirementsSpecificationModel = fixture.ServiceProvider.GetRequiredService<RequirementsSpecificationModel>();
        _testCasesModel = fixture.ServiceProvider.GetRequiredService<TestCasesModel>();
        _cyclesModel = fixture.ServiceProvider.GetRequiredService<CyclesModel>();
        _testPlansModel = fixture.ServiceProvider.GetRequiredService<TestPlansModel>();
    }

    [Fact]
    public void Sample()
    {
        
    }
}
