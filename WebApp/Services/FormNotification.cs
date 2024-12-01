using Radzen;

namespace WebApp.Services;

public class FormNotificationService(NotificationService notificationService)
{
    public Task NotifySuccess(string message)
    {
        return Task.Run(() =>
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Success,
                Summary = "Success",
                Detail = message,
                Duration = 4000,
            });
        });
    }

    public Task NotifyError(string message)
    {
        return Task.Run(() =>
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Error,
                Summary = "Error",
                Detail = message,
                Duration = 4000
            });
        });
    }

    public Task NotifyWarning(string message)
    {
        return Task.Run(() =>
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Warning,
                Summary = "Warning",
                Detail = message,
                Duration = 4000
            });
        });
    }

    public Task NotifyInfo(string message)
    {
        return Task.Run(() =>
        {
            notificationService.Notify(new NotificationMessage
            {
                Severity = NotificationSeverity.Info,
                Summary = "Info",
                Detail = message,
                Duration = 4000
            });
        });
    }
}