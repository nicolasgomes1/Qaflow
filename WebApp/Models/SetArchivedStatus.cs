using WebApp.Data;
using WebApp.Data.enums;

namespace WebApp.Models;

public static class SetArchivedStatus
{
    public static T SetArchivedStatusBasedOnWorkflow<T>(T value)
    {
        var type = typeof(T);

        // Use nameof to get property names safely
        var workflowStatusProp = type.GetProperty(nameof(Requirements.WorkflowStatus));
        var archivedStatusProp = type.GetProperty(nameof(Requirements.ArchivedStatus));

        if (workflowStatusProp != null && archivedStatusProp != null)
        {
            var workflowStatusValue = workflowStatusProp.GetValue(value);

            // Use enum comparison instead of string
            if (workflowStatusValue != null && workflowStatusValue.Equals(WorkflowStatus.Completed))
                // Set ArchivedStatus to Archived
                archivedStatusProp.SetValue(value, ArchivedStatus.Archived);
        }

        return value;
    }
}