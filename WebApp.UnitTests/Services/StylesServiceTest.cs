using Bunit;
using JetBrains.Annotations;
using Radzen;
using WebApp.Components.Pages.TestExecutions.Components;
using WebApp.Data;
using WebApp.Data.enums;
using WebApp.Services;
using WebApp.UnitTests.BaseTest;
using WebApp.UnitTests.DIContainers;

namespace WebApp.UnitTests.Services;

[TestSubject(typeof(StylesService))]
public class StylesServiceTest : BaseComponentTest
{
    public StylesServiceTest(TestFixture fixture) : base(fixture)
    {
    }



    [Theory]
    [InlineData(ExecutionStatus.Failed, BadgeStyle.Danger)]
    [InlineData(ExecutionStatus.Passed, BadgeStyle.Success)]
    [InlineData(ExecutionStatus.NotRun, BadgeStyle.Info)]
    public void GetBadgeStyleExecution_ShouldReturnCorrectBadgeStyle(ExecutionStatus status, BadgeStyle expectedStyle)
    {
        // Act
        var result = StylesService.GetBadgeStyleExecution(status);

        // Assert
        Assert.Equal(expectedStyle, result);
    }

    [Theory]
    [InlineData(Priority.Critical, BadgeStyle.Danger)]
    [InlineData(Priority.High, BadgeStyle.Warning)]
    [InlineData(Priority.Medium, BadgeStyle.Info)]
    [InlineData(Priority.Low, BadgeStyle.Primary)]
    public void GetBadgeStylePriority_ShouldReturnCorrectBadgeStyle(Priority priority, BadgeStyle expectedStyle)
    {
        // Act
        var result = StylesService.GetBadgeStylePriority(priority);

        // Assert
        Assert.Equal(expectedStyle, result);
    }

    [Theory]
    [InlineData(WorkflowStatus.New, BadgeStyle.Info)]
    [InlineData(WorkflowStatus.InReview, BadgeStyle.Warning)]
    [InlineData(WorkflowStatus.Completed, BadgeStyle.Success)]
    public void GetBadgeStyleWorkflowStatus_ShouldReturnCorrectBadgeStyle(WorkflowStatus status, BadgeStyle expectedStyle)
    {
        // Act
        var result = StylesService.GetBadgeStyleWorkflowStatus(status);

        // Assert
        Assert.Equal(expectedStyle, result);
    }

    [Theory]
    [InlineData(ArchivedStatus.Active, BadgeStyle.Success)]
    [InlineData(ArchivedStatus.Archived, BadgeStyle.Base)]
    public void GetBadgeStyleArchivedStatus_ShouldReturnCorrectBadgeStyle(ArchivedStatus status, BadgeStyle expectedStyle)
    {
        // Act
        var result = StylesService.GetBadgeStyleArchivedStatus(status);

        // Assert
        Assert.Equal(expectedStyle, result);
    }
    
    [Theory]
    [InlineData(ExecutionStatus.Failed, "rz-badge-danger")]
    [InlineData(ExecutionStatus.Passed, "rz-badge-success")]
    [InlineData(ExecutionStatus.NotRun, "rz-badge-info")]
    public void GetBadgeStyleExecution_ReturnsCorrectStyle(ExecutionStatus status, string expectedClass)
    {
        // Act
        var badgeStyle = StylesService.GetBadgeStyleExecution(status);

        // Assert
        Assert.Equal(expectedClass, $"rz-badge-{badgeStyle.ToString().ToLower()}");
    }

    [Theory]
    [InlineData(Priority.Critical, "rz-badge-danger")]
    [InlineData(Priority.High, "rz-badge-warning")]
    [InlineData(Priority.Medium, "rz-badge-info")]
    [InlineData(Priority.Low, "rz-badge-primary")]
    public void GetBadgeStylePriority_ReturnsCorrectStyle(Priority priority, string expectedClass)
    {
        // Act
        var badgeStyle = StylesService.GetBadgeStylePriority(priority);

        // Assert
        Assert.Equal(expectedClass, $"rz-badge-{badgeStyle.ToString().ToLower()}");
    }

    [Theory]
    [InlineData(WorkflowStatus.New, "rz-badge-info")]
    [InlineData(WorkflowStatus.InReview, "rz-badge-warning")]
    [InlineData(WorkflowStatus.Completed, "rz-badge-success")]
    public void GetBadgeStyleWorkflowStatus_ReturnsCorrectStyle(WorkflowStatus status, string expectedClass)
    {
        // Act
        var badgeStyle = StylesService.GetBadgeStyleWorkflowStatus(status);

        // Assert
        Assert.Equal(expectedClass, $"rz-badge-{badgeStyle.ToString().ToLower()}");
    }

    [Theory]
    [InlineData(ArchivedStatus.Active, "rz-badge-success")]
    [InlineData(ArchivedStatus.Archived, "rz-badge-base")]
    public void GetBadgeStyleArchivedStatus_ReturnsCorrectStyle(ArchivedStatus status, string expectedClass)
    {
        // Act
        var badgeStyle = StylesService.GetBadgeStyleArchivedStatus(status);

        // Assert
        Assert.Equal(expectedClass, $"rz-badge-{badgeStyle.ToString().ToLower()}");
    }

}
