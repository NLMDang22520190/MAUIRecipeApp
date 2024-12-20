using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MailKit.Net.Smtp;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MAUIRecipeApp.Service
{
    public class MailService
    {
        private static MailService _instance; 
        private static readonly object _lock = new object();

        public static MailService Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MailService();
                    }
                    return _instance;
                }
            }
        }

        private readonly string _smtpServer = "smtp.gmail.com"; // Thay bằng SMTP server bạn sử dụng
        private readonly int _smtpPort = 587; // Cổng SMTP
        private readonly string _emailFrom = "kiseryouta2003@gmail.com"; // Email của bạn
        private readonly string _emailPassword = "qcqa slmu vkbr edha"; // Mật khẩu ứng dụng

        public async Task<string> SendVerificationCodeAsync(string emailTo)
        {
            // Tạo mã xác nhận 6 chữ số
            var random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();

            // Tạo nội dung email
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("KitchenVerse", _emailFrom));
            message.To.Add(new MailboxAddress("", emailTo));
            message.Subject = "Your verification code";

            message.Body = new TextPart("plain")
            {
                Text = $"Your verification code is: {verificationCode}"
            };

            // Gửi email
            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    await client.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(_emailFrom, _emailPassword);
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Không thể gửi email.", ex);
                }
            }

            // Trả về mã xác nhận
            return verificationCode;
        }
    }
}
