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
    private string _fileContent = string.Empty;

    public ManageCsvUpload(FormNotificationService formNotificationService, ProjectModel projectModel,
        DialogService dialogService, RequirementsModel requirementsModel,
        RequirementsSpecificationModel requirementsSpecificationModel)
    {
        _formNotificationService = formNotificationService;
        _projectModel = projectModel;
        _dialogService = dialogService;
        _requirementsModel = requirementsModel;
        _requirementsSpecificationModel = requirementsSpecificationModel;
    }

    private IEnumerable<Projects> _projects = [];
    private IEnumerable<Requirements> _requirements = [];
    private IEnumerable<RequirementsSpecification> _requirementsSpecifications = [];


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
}