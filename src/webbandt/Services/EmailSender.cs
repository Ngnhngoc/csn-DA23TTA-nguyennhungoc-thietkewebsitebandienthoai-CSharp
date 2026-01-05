using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Cấu hình SMTP Client
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            // LƯU Ý: Password ở đây phải là MẬT KHẨU ỨNG DỤNG (App Password), không phải mật khẩu đăng nhập Gmail
            Credentials = new NetworkCredential("ngocsummer0309@gmail.com", "mfmr aurx sqcm xihf")
        };

        // Tạo nội dung email
        var mailMessage = new MailMessage
        {
            From = new MailAddress("email_cua_ban@gmail.com"),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        mailMessage.To.Add(email);

        // Gửi mail
        return client.SendMailAsync(mailMessage);
    }
}