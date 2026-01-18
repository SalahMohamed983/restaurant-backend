using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantBusinessLayer.Settings;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _settings;
        private readonly string? _smtpHost;
        private readonly int _smtpPort;
        private readonly string? _smtpUser;
        private readonly string? _smtpPassword;
        private readonly string? _smtpFromEmail;
        private readonly string? _smtpFromName;
        private readonly bool _enableSsl;
        private readonly string? _baseUrl;

        public EmailService(IOptions<EmailSettings> options, ILogger<EmailService> logger)
        {
            _settings = options.Value;
            _logger = logger;

            _smtpHost = _settings.Smtp.Host;
            _smtpPort = _settings.Smtp.Port;
            _smtpUser = _settings.Smtp.User;
            _smtpPassword = _settings.Smtp.Password;
            _smtpFromEmail = !string.IsNullOrEmpty(_settings.Smtp.FromEmail) ? _settings.Smtp.FromEmail : _smtpUser;
            _smtpFromName = !string.IsNullOrEmpty(_settings.Smtp.FromName) ? _settings.Smtp.FromName : "Resturant API";
            _enableSsl = _settings.Smtp.EnableSsl;
            _baseUrl = _settings.BaseUrl;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                // If SMTP is not configured, just log and return true (for development)
                if (string.IsNullOrEmpty(_smtpHost) || string.IsNullOrEmpty(_smtpUser))
                {
                    _logger.LogWarning("Email not configured. Would send email to {Email}: {Subject}", to, subject);
                    _logger.LogInformation("Email Body: {Body}", body);
                    return true; // Return true in development mode
                }

                using var client = new SmtpClient(_smtpHost, _smtpPort)
                {
                    EnableSsl = _enableSsl,
                    Credentials = new NetworkCredential(_smtpUser, _smtpPassword)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_smtpFromEmail!, _smtpFromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(new MailAddress(to));

                await client.SendMailAsync(message);
                _logger.LogInformation("Email sent successfully to {Email}", to);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", to);
                return false;
            }
        }

        public async Task<bool> SendEmailConfirmationAsync(string email, string confirmationLink)
        {
            var subject = "تأكيد بريدك الإلكتروني - Restaurant API";
            var body = $@"
                <div dir='rtl' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>مرحباً بك في Restaurant API</h2>
                    <p>شكراً لك على التسجيل! يرجى تأكيد بريدك الإلكتروني بالنقر على الرابط أدناه:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            تأكيد البريد الإلكتروني
                        </a>
                    </p>
                    <p>أو يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>
                    <p style='word-break: break-all; color: #666;'>{confirmationLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        إذا لم تقم بالتسجيل في موقعنا، يمكنك تجاهل هذه الرسالة.
                    </p>
                </div>
                <div dir='ltr' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Welcome to Restaurant API</h2>
                    <p>Thank you for registering! Please confirm your email address by clicking the link below:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Confirm Email Address
                        </a>
                    </p>
                    <p>Or you can copy and paste the following link into your browser:</p>
                    <p style='word-break: break-all; color: #666;'>{confirmationLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        If you didn't sign up for our service, you can safely ignore this email.
                    </p>
                </div>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendPasswordResetAsync(string email, string resetLink)
        {
            var subject = "إعادة تعيين كلمة المرور - Restaurant API";
            var body = $@"
                <div dir='rtl' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>إعادة تعيين كلمة المرور</h2>
                    <p>لقد طلبت إعادة تعيين كلمة المرور. يرجى النقر على الرابط أدناه لإعادة تعيين كلمة المرور:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #dc3545; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            إعادة تعيين كلمة المرور
                        </a>
                    </p>
                    <p>أو يمكنك نسخ الرابط التالي ولصقه في المتصفح:</p>
                    <p style='word-break: break-all; color: #666;'>{resetLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        إذا لم تطلب إعادة تعيين كلمة المرور، يمكنك تجاهل هذه الرسالة.
                    </p>
                </div>
                <div dir='ltr' style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333;'>Password Reset</h2>
                    <p>You have requested to reset your password. Please click the link below to reset your password:</p>
                    <p style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #dc3545; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Reset Password
                        </a>
                    </p>
                    <p>Or you can copy and paste the following link into your browser:</p>
                    <p style='word-break: break-all; color: #666;'>{resetLink}</p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        If you didn't request a password reset, you can safely ignore this email.
                    </p>
                </div>";

            return await SendEmailAsync(email, subject, body);
        }
    }
}
