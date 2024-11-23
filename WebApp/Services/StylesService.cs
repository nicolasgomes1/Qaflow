using Radzen;
using WebApp.Data.enums;

namespace WebApp.Services;

public class StylesService
{
    // Method to get the style based on ExecutionStatus
    private static string GetRowStyle(ExecutionStatus status)
    {
        return status switch
        {
            ExecutionStatus.Passed => "background-color: lightgreen; color: white;",
            ExecutionStatus.Failed => "background-color: #FFCCCB; color: white;",
            ExecutionStatus.NotRun => "background-color: gray; color: white;",
            _ => string.Empty
        };
    }

    // General method to apply style to a Radzen row
    public static void ApplyRowStyle<T>(Radzen.RowRenderEventArgs<T> args, ExecutionStatus status)
    {
        var style = GetRowStyle(status);
        if (!string.IsNullOrEmpty(style))
        {
            args.Attributes.Add("style", style);
        }
    }

    public static BadgeStyle GetBadgeStyleExecution(ExecutionStatus executionStatus)
    {
        return executionStatus switch
        {
            ExecutionStatus.Failed => BadgeStyle.Danger,
            ExecutionStatus.Passed => BadgeStyle.Success,
            _ => BadgeStyle.Info
        };
    }

    public static BadgeStyle GetBadgeStylePriority(Priority priority)
    {
        return priority switch
        {
            Priority.Critical => BadgeStyle.Danger,
            Priority.High => BadgeStyle.Warning,
            Priority.Medium => BadgeStyle.Info,
            Priority.Low => BadgeStyle.Primary,
            _ => BadgeStyle.Secondary
        };
    }

    public static BadgeStyle GetBadgeStyleSeverity(Severity severity)
    {
        return severity switch
        {
            Severity.Critical => BadgeStyle.Danger,
            Severity.High => BadgeStyle.Warning,
            Severity.Medium => BadgeStyle.Info,
            Severity.Low => BadgeStyle.Primary,
            _ => BadgeStyle.Secondary
        };
    }

    public static BadgeStyle GetBadgeStyleBugStatus(BugStatus bugStatus)
    {
        return bugStatus switch
        {
            BugStatus.Open => BadgeStyle.Info,
            BugStatus.InReview => BadgeStyle.Primary,
            BugStatus.InProgress => BadgeStyle.Warning,
            BugStatus.Closed => BadgeStyle.Success,
            _ => BadgeStyle.Secondary
        };
    }
}