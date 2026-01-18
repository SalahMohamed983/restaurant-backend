namespace ResturantBusinessLayer.Settings
{
    public class EmailSettings
    {
        public string BaseUrl { get; set; } = "https://localhost:5001";
        public SmtpSettings Smtp { get; set; } = new SmtpSettings();
    }

    public class SmtpSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string FromName { get; set; } = "Resturant API";
        public bool EnableSsl { get; set; } = true;
    }
}
