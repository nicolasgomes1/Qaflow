using JetBrains.Annotations;
using Radzen;
using WebApp.Services;

namespace WebApp.UnitTests.Services;

[TestSubject(typeof(FormNotificationService))]
public class FormNotificationServiceTest
{

    [Fact]
    public async Task TestNotifySuccess()
    {
        // Arrange
        var message = "Test success message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifySuccess(message);

        // Assert
        Assert.Single(notificationService.Messages);
        var notification = notificationService.Messages.First();
        Assert.Equal(NotificationSeverity.Success, notification.Severity);
        Assert.Equal("Success", notification.Summary);
        Assert.Equal(message, notification.Detail);
        Assert.Equal(4000, notification.Duration);
    }
    
    [Fact]
    public async Task TestNotifyError()
    {
        // Arrange
        var message = "Test error message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyError(message);

        // Assert
        Assert.Single(notificationService.Messages);
        var notification = notificationService.Messages.First();
        Assert.Equal(NotificationSeverity.Error, notification.Severity);
        Assert.Equal("Error", notification.Summary);
        Assert.Equal(message, notification.Detail);
        Assert.Equal(4000, notification.Duration);
    }
    
    [Fact]
    public async Task TestNotifyWarning()
    {
        // Arrange
        var message = "Test warning message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyWarning(message);

        // Assert
        Assert.Single(notificationService.Messages);
        var notification = notificationService.Messages.First();
        Assert.Equal(NotificationSeverity.Warning, notification.Severity);
        Assert.Equal("Warning", notification.Summary);
        Assert.Equal(message, notification.Detail);
        Assert.Equal(4000, notification.Duration);
    }
    
    [Fact]
    public async Task TestNotifyInfo()
    {
        // Arrange
        var message = "Test info message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyInfo(message);

        // Assert
        Assert.Single(notificationService.Messages);
        var notification = notificationService.Messages.First();
        Assert.Equal(NotificationSeverity.Info, notification.Severity);
        Assert.Equal("Info", notification.Summary);
        Assert.Equal(message, notification.Detail);
        Assert.Equal(4000, notification.Duration);
    }
}