using Radzen;
using WebApp.Services;

namespace Tests.Services;

[TestClass]
public class FormNotificationServiceTest
{

    [TestMethod]
    public async Task TestNotifySuccess()
    {
        // Arrange
        var message = "Test success message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifySuccess(message);

        // Assert
        Assert.AreEqual(1, notificationService.Messages.Count);
        var notification = notificationService.Messages.First();
        Assert.AreEqual(NotificationSeverity.Success, notification.Severity);
        Assert.AreEqual("Success", notification.Summary);
        Assert.AreEqual(message, notification.Detail);
        Assert.AreEqual(4000, notification.Duration);
    }
    
    [TestMethod]
    public async Task TestNotifyError()
    {
        // Arrange
        var message = "Test error message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyError(message);

        // Assert
        Assert.AreEqual(1, notificationService.Messages.Count);
        var notification = notificationService.Messages.First();
        Assert.AreEqual(NotificationSeverity.Error, notification.Severity);
        Assert.AreEqual("Error", notification.Summary);
        Assert.AreEqual(message, notification.Detail);
        Assert.AreEqual(4000, notification.Duration);
    }
    
    [TestMethod]
    public async Task TestNotifyWarning()
    {
        // Arrange
        var message = "Test warning message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyWarning(message);

        // Assert
        Assert.AreEqual(1, notificationService.Messages.Count);
        var notification = notificationService.Messages.First();
        Assert.AreEqual(NotificationSeverity.Warning, notification.Severity);
        Assert.AreEqual("Warning", notification.Summary);
        Assert.AreEqual(message, notification.Detail);
        Assert.AreEqual(4000, notification.Duration);
    }
    
    [TestMethod]
    public async Task TestNotifyInfo()
    {
        // Arrange
        var message = "Test info message";
        var notificationService = new NotificationService();
        var formNotificationService = new FormNotificationService(notificationService);

        // Act
        await formNotificationService.NotifyInfo(message);

        // Assert
        Assert.AreEqual(1, notificationService.Messages.Count);
        var notification = notificationService.Messages.First();
        Assert.AreEqual(NotificationSeverity.Info, notification.Severity);
        Assert.AreEqual("Info", notification.Summary);
        Assert.AreEqual(message, notification.Detail);
        Assert.AreEqual(4000, notification.Duration);
    }
}