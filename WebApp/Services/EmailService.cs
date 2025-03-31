using System.Net;
using System.Net.Mail;

namespace WebApp.Services;

public interface IEmailService
{
    //Task SendEmailAsync(string toEmail, string subject, string body);
    Task SendUserEmailLinkAsync(string email, string subject, string confirmlink);
}

public class EmailService : IEmailService
{
    private readonly SmtpClient _smtpClient = new("smtp.gmail.com")
    {
        Port = 587, // Replace with your SMTP port
        Credentials = new NetworkCredential("testdotnet10@gmail.com", "rmrcmrpqgkcrfkgl"),
        EnableSsl = true
    };

    // Replace with your SMTP port


    private async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress("testdotnet10@gmail.com"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        mailMessage.To.Add(toEmail);

        await _smtpClient.SendMailAsync(mailMessage);
    }

    public async Task SendUserEmailLinkAsync(string email, string subject, string confirmlink)
    {
        var message = new MailMessage();
        var smtpClient = new SmtpClient();
        message.From = new MailAddress("testdotnet10@gmail.com");
        message.To.Add(email);
        message.Subject = subject;
        message.IsBodyHtml = true;
        message.Body = confirmlink;
        smtpClient.Port = 587;
        smtpClient.Host = "smtp.gmail.com";
        smtpClient.EnableSsl = true;
        smtpClient.UseDefaultCredentials = false;
        smtpClient.Credentials = new NetworkCredential("testdotnet10@gmail.com", "rmrcmrpqgkcrfkgl");
        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        await smtpClient.SendMailAsync(message);
    }
}