using Microsoft.AspNetCore.Components;
using Radzen;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Components.ReusableComponents.UploadCsv;

public class ManageCsvUpload
{
    private readonly FormNotificationService _formNotificationService;
    private readonly ProjectModel _projectModel;
    private readonly RequirementsModel _requirementsModel;
    private readonly DialogService _dialogService;
    private readonly RequirementsSpecificationModel _requirementsSpecificationModel;
    private readonly TestCasesModel _testCasesModel;
    private readonly CyclesModel _cyclesModel;
    private readonly TestPlansModel _testPlansModel;
    private string _fileContent = string.Empty;

    public ManageCsvUpload(FormNotificationService formNotificationService, ProjectModel projectModel,
        DialogService dialogService, RequirementsModel requirementsModel,
        RequirementsSpecificationModel requirementsSpecificationModel, TestCasesModel testCasesModel,
        TestPlansModel testPlansModel, CyclesModel cyclesModel)
    {
        _formNotificationService = formNotificationService;
        _projectModel = projectModel;
        _dialogService = dialogService;
        _requirementsModel = requirementsModel;
        _requirementsSpecificationModel = requirementsSpecificationModel;
        _testCasesModel = testCasesModel;
        _testPlansModel = testPlansModel;
        _cyclesModel = cyclesModel;
    }

    private IEnumerable<Projects> _projects = [];
    private IEnumerable<Requirements> _requirements = [];
    private IEnumerable<RequirementsSpecification> _requirementsSpecifications = [];
    private IEnumerable<TestPlans> _testPlans = [];
    private IEnumerable<TestCases> _testCases = [];
    private IEnumerable<Cycles> _cycles = [];


    int _currentLine = 0;
    int _validLines;
    int _invalidLines;
    int _totalLines;


    public async Task SaveFileContent(UploadChangeEventArgs args)
    {
        if (!args.Files.Any()) throw new Exception("No files found.");

        var file = args.Files.First();
        using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        using var reader = new StreamReader(memoryStream);
        _fileContent = await reader.ReadToEndAsync();

        if (string.IsNullOrWhiteSpace(_fileContent))
        {
            await _formNotificationService.NotifyError("No file content found. Please upload a CSV file.");
        }
    }


    public async Task ProcessProjectsCsvFile(EventCallback callback)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _projects = await _projectModel.GetProjects();

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            if (values.Length < 2)
            {
                _invalidLines++;
                continue;
            }

            var projects = new Projects
            {
                Name = values[0].Trim(),
                Description = values[1].Trim()
            };

            if (string.IsNullOrWhiteSpace(projects.Name) || string.IsNullOrWhiteSpace(projects.Description))
            {
                _invalidLines++;
                continue;
            }

            if (_projects.Any(r => r.Name == projects.Name && r.Description == projects.Description))
            {
                _invalidLines++;
                continue;
            }

            await _projectModel.AddProjectFromCsv(projects);
            _validLines++;
        }

        _dialogService.Close();
        await callback.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError($"No {nameof(Projects)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(Projects)} CSV imported successfully with {_validLines} projects and {_invalidLines} invalid lines.");
        }
    }

    public async Task ProcessRequirementsCsvFile(EventCallback onUploadCompleted, int projectId)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _requirements = await _requirementsModel.GetRequirementsToValidateAgainstCsv(projectId);

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            if (values.Length < 2)
            {
                _invalidLines++;
                continue;
            }

            var requirements = new Requirements
            {
                Name = values[0].Trim(),
                Description = values[1].Trim(),
                Priority = Enum.Parse<Priority>(values[2].Trim())
            };

            if (string.IsNullOrWhiteSpace(requirements.Name) || string.IsNullOrWhiteSpace(requirements.Description))
            {
                _invalidLines++;
                continue;
            }

            if (_requirements.Any(r => r.Name == requirements.Name && r.Description == requirements.Description))
            {
                _invalidLines++;
                continue;
            }

            await _requirementsModel.AddRequirementFromCsv(requirements, projectId);
            _validLines++;
        }

        _dialogService.Close();
        await onUploadCompleted.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError($"No {nameof(Requirements)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(Requirements)} CSV imported successfully with {_validLines} {nameof(Requirements)} and {_invalidLines} invalid lines.");
        }
    }

    public async Task ProcessRequirementsSpecificationCsvFile(EventCallback onUploadCompleted, int projectId)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _requirementsSpecifications =
            await _requirementsSpecificationModel.GetRequirementsSpecificationListAsync(projectId);

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            if (values.Length < 2)
            {
                _invalidLines++;
                continue;
            }

            var requirmentsSpecification = new RequirementsSpecification
            {
                Name = values[0].Trim(),
                Description = values[1].Trim()
            };

            if (string.IsNullOrWhiteSpace(requirmentsSpecification.Name) ||
                string.IsNullOrWhiteSpace(requirmentsSpecification.Description))
            {
                _invalidLines++;
                continue;
            }

            if (_requirementsSpecifications.Any(r =>
                    r.Name == requirmentsSpecification.Name && r.Description == requirmentsSpecification.Description))
            {
                _invalidLines++;
                continue;
            }

            await _requirementsSpecificationModel.AddRequirementsSpecificationFromCsv(requirmentsSpecification,
                projectId);
            _validLines++;
        }

        _dialogService.Close();
        await onUploadCompleted.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError(
                $"No {nameof(RequirementsSpecificationModel)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(RequirementsSpecificationModel)} CSV imported successfully with {_validLines} requirements specifications and {_invalidLines} invalid lines.");
        }
    }

    public async Task ProcessTestPlansCsvFile(EventCallback onUploadCompleted, int projectId)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _testPlans = await _testPlansModel.GetTestPlans(projectId);

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            if (values.Length < 2)
            {
                _invalidLines++;
                continue;
            }

            var testplans = new TestPlans
            {
                Name = values[0].Trim(),
                Description = values[1].Trim(),
                Priority = Enum.Parse<Priority>(values[2].Trim()),
                Cycle = new Cycles { Name = values[3].Trim() }
            };

            if (string.IsNullOrWhiteSpace(testplans.Name) || string.IsNullOrWhiteSpace(testplans.Description))
            {
                _invalidLines++;
                continue;
            }

            if (_testPlans.Any(r => r.Name == testplans.Name && r.Description == testplans.Description))
            {
                _invalidLines++;
                continue;
            }

            await _testPlansModel.AddTestPlanFromCsv(testplans, projectId);
            _validLines++;
        }

        _dialogService.Close();
        await onUploadCompleted.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError($"No {nameof(TestPlans)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(TestPlans)} CSV imported successfully with {_validLines} test plans and {_invalidLines} invalid lines.");
        }
    }

    public async Task ProcessTestCasesCsvFile(EventCallback onUploadCompleted, int projectId)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _testCases = await _testCasesModel.GetTestCasesToValidateAgainstCsv(projectId);

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            if (values.Length < 2)
            {
                _invalidLines++;
                continue;
            }

            var testCases = new TestCases
            {
                Name = values[0].Trim(),
                Description = values[1].Trim(),
                Priority = Enum.Parse<Priority>(values[2].Trim()),
                TestType = Enum.Parse<TestTypes>(values[3].Trim()),
                TestScope = Enum.Parse<TestScope>(values[4].Trim())
            };

            if (string.IsNullOrWhiteSpace(testCases.Name) || string.IsNullOrWhiteSpace(testCases.Description))
            {
                _invalidLines++;
                continue;
            }

            if (_testCases.Any(r => r.Name == testCases.Name && r.Description == testCases.Description))
            {
                _invalidLines++;
                continue;
            }

            await _testCasesModel.AddTestCasesFromCsv(testCases, projectId);
            _validLines++;
        }

        _dialogService.Close();
        await onUploadCompleted.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError($"No {nameof(TestCases)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(TestCases)} CSV imported successfully with {_validLines} test cases and {_invalidLines} invalid lines.");
        }
    }

    public async Task ProcessCyclesCsvFile(EventCallback onUploadCompleted, int projectId)
    {
        var csvlines = _fileContent.Split('\n').Skip(1);
        _totalLines = csvlines.Count();

        _cycles = await _cyclesModel.GetCyclesByProjectId(projectId);

        _currentLine = 0;
        _validLines = 0;
        _invalidLines = 0;

        foreach (var line in csvlines)
        {
            _currentLine++;

            if (string.IsNullOrWhiteSpace(line))
            {
                _invalidLines++;
                continue;
            }

            var values = line.Split(',');
            // Require Name, StartDate, EndDate
            if (values.Length < 3)
            {
                _invalidLines++;
                continue;
            }

            // Parse dates safely; if parsing fails, mark as invalid
            var name = values[0].Trim();
            if (!DateTime.TryParse(values[1].Trim(), out var parsedStart))
            {
                _invalidLines++;
                continue;
            }

            if (!DateTime.TryParse(values[2].Trim(), out var parsedEnd))
            {
                _invalidLines++;
                continue;
            }

            var cycles = new Cycles
            {
                Name = name,
                // Keep behavior consistent with existing code: mark as UTC without conversion
                StartDate = DateTime.SpecifyKind(parsedStart, DateTimeKind.Utc),
                EndDate = DateTime.SpecifyKind(parsedEnd, DateTimeKind.Utc)
            };

            if (string.IsNullOrWhiteSpace(cycles.Name))
            {
                _invalidLines++;
                continue;
            }

            // Start must be strictly earlier than End
            if (cycles.StartDate >= cycles.EndDate)
            {
                _invalidLines++;
                continue;
            }

            if (_cycles.Any(r => r.Name == cycles.Name))
            {
                _invalidLines++;
                continue;
            }

            await _cyclesModel.AddCyclesFromCsv(cycles, projectId);
            _validLines++;
        }

        _dialogService.Close();
        await onUploadCompleted.InvokeAsync();

        if (_validLines == 0)
        {
            await _formNotificationService.NotifyError($"No {nameof(Cycles)} found in the CSV file.");
        }
        else
        {
            await _formNotificationService.NotifySuccess(
                $"{nameof(Cycles)} CSV imported successfully with {_validLines} cycles and {_invalidLines} invalid lines.");
        }
    }
}