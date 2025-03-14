namespace Tests;

[TestClass]
public class IntegrationTest
{
     [TestMethod]
     public async Task GetWebResourceRootReturnsOkStatusCode()
     {
         // Arrange
         var appHost = await DistributedApplicationTestingBuilder.CreateAsync<Projects.WebApp>();
         appHost.Services.ConfigureHttpClientDefaults(clientBuilder =>
         {
             clientBuilder.AddStandardResilienceHandler();
         });
         await using var app = await appHost.BuildAsync();
         var resourceNotificationService = app.Services.GetRequiredService<ResourceNotificationService>();
         await app.StartAsync();

         // Act
         var httpClient = app.CreateHttpClient("webapp");
         await resourceNotificationService.WaitForResourceAsync("webapp", KnownResourceStates.Running).WaitAsync(TimeSpan.FromSeconds(30));
         var response = await httpClient.GetAsync("/");

         // Assert
         Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
     }
}