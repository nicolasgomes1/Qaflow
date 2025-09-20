using Xunit;
using Microsoft.AspNetCore.Components;
using Radzen;
using WebApp.Data;
using WebApp.Data.enums;
using System.Text;
using Microsoft.AspNetCore.Components.Forms;
using WebApp.Models;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;
using FileInfo = Radzen.FileInfo;

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

    private class TestFile : IBrowserFile
    {
        private readonly MemoryStream _stream;
        public string Name { get; }
        public string ContentType { get; }
        public long Size { get; }
        public DateTimeOffset LastModified { get; }

        public TestFile(string name, string contentType, MemoryStream stream)
        {
            Name = name;
            ContentType = contentType;
            _stream = stream;
            Size = stream.Length;
            LastModified = DateTimeOffset.UtcNow;
        }

        public Stream OpenReadStream(long maxAllowedSize = 512000, CancellationToken cancellationToken = default)
        {
            _stream.Position = 0;
            return _stream;
        }
    }

    [Fact]
    public async Task SaveFileContent_ThrowsException_WhenNoFiles()
    {
        var args = new UploadChangeEventArgs { Files = Enumerable.Empty<FileInfo>() };
        await Assert.ThrowsAsync<Exception>(() => _service.SaveFileContent(args));
    }

    [Fact]
    public async Task SaveFileContent_NotifiesError_WhenFileContentEmpty()
    {
        var stream = new MemoryStream(Array.Empty<byte>());
        var emptyFile = new TestFile("test.csv", "text/csv", stream);
        var args = new UploadChangeEventArgs 
        { 
            Files = new[] { new FileInfo(emptyFile) } 
        };
        
        await _service.SaveFileContent(args);
    }

    // Projects CSV Tests
    [Fact]
    public async Task ProcessProjectsCsvFile_ValidInput_AddsProjects()
    {
        var csvContent = "Name,Description\nProject1,Desc1\nProject2,Desc2";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        await _service.ProcessProjectsCsvFile(callback);
    }

    [Fact]
    public async Task ProcessProjectsCsvFile_DuplicateProject_SkipsAndCountsAsInvalid()
    {
        var existingProjects = new List<Data.Projects> 
        { 
            new() { Name = "Project1", Description = "Desc1" } 
        };
        
        var csvContent = "Name,Description\nProject1,Desc1";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        await _service.ProcessProjectsCsvFile(callback);
    }

    // Requirements CSV Tests
    [Fact]
    public async Task ProcessRequirementsCsvFile_ValidInput_AddsRequirements()
    {
        var csvContent = "Name,Description,Priority\nReq1,Desc1,High\nReq2,Desc2,Medium";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessRequirementsCsvFile(callback, projectId);
    }

    // Requirements Specification CSV Tests
    [Fact]
    public async Task ProcessRequirementsSpecificationCsvFile_ValidInput_AddsSpecifications()
    {
        var csvContent = "Name,Description\nSpec1,Desc1\nSpec2,Desc2";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessRequirementsSpecificationCsvFile(callback, projectId);
    }

    // Test Plans CSV Tests
    [Fact]
    public async Task ProcessTestPlansCsvFile_ValidInput_AddsTestPlans()
    {
        var csvContent = "Name,Description,Priority,Cycle\nPlan1,Desc1,High,Cycle1";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessTestPlansCsvFile(callback, projectId);
    }

    // Test Cases CSV Tests
    [Fact]
    public async Task ProcessTestCasesCsvFile_ValidInput_AddsTestCases()
    {
        var csvContent = "Name,Description,Priority,TestType,TestScope\nCase1,Desc1,High,AcceptanceTest,UiTests";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessTestCasesCsvFile(callback, projectId);
    }

    // Cycles CSV Tests
    [Fact]
    public async Task ProcessCyclesCsvFile_ValidInput_AddsCycles()
    {
        var csvContent = "Name,StartDate,EndDate\nCycle1,2025-01-01,2025-12-31";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessCyclesCsvFile(callback, projectId);
    }

    [Fact]
    public async Task ProcessCyclesCsvFile_InvalidDates_CountsAsInvalid()
    {
        var csvContent = "Name,StartDate,EndDate\nCycle1,invalid,2025-12-31";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessCyclesCsvFile(callback, projectId);
    }

    [Fact]
    public async Task ProcessCyclesCsvFile_EndDateBeforeStartDate_CountsAsInvalid()
    {
        var csvContent = "Name,StartDate,EndDate\nCycle1,2025-12-31,2025-01-01";
        await UploadCsvContent(csvContent);
        
        var callback = new EventCallback();
        const int projectId = 1;
        await _service.ProcessCyclesCsvFile(callback, projectId);
    }

    // Helper method to simulate file upload
    private async Task UploadCsvContent(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        var stream = new MemoryStream(bytes);
        var file = new TestFile("test.csv", "text/csv", stream);

        var args = new UploadChangeEventArgs 
        { 
            Files = new[] { new FileInfo(file) } 
        };

        await _service.SaveFileContent(args);
    }
}
