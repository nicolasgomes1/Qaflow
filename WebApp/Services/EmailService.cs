using System.Net;
using System.Net.Mail;

namespace WebApp.Services;

public interface IEmailService
{
    Task SendEmailAsync(string toEmail, string subject, string body);
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


    public async Task SendEmailAsync(string toEmail, string subject, string body)
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
}