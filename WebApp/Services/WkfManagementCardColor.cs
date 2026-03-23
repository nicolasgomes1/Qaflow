using Radzen;
using WebApp.Data.enums;

namespace WebApp.Services;

public static class WkfManagementCardColor
{
    public static void OnItemRender<T>(RadzenDropZoneItemRenderEventArgs<T> args)
    {
        var workflowStatus = (WorkflowStatus)(typeof(T)
            .GetProperty("WorkflowStatus")?
            .GetValue(args.Item) ?? null!);

        args.Attributes["class"] =
            "rz-card rz-variant-filled rz-background-color-primary-light rz-color-on-primary-light";

        args.Attributes["class"] += workflowStatus switch
        {
            WorkflowStatus.New => " rz-border-danger",
            WorkflowStatus.InReview => " rz-border-warning",
            WorkflowStatus.Completed => " rz-border-success",
            WorkflowStatus.Reopened => " rz-border-primary",
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}