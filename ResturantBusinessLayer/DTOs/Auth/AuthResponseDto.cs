namespace ResturantBusinessLayer.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserInfoDto User { get; set; } = null!;
    }

    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class GoogleLoginDto
    {
        public string Code { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
    }

    internal record GoogleUserInfo(
        string Email,
        bool EmailVerified,
        string? Name,
        string? Picture,
        string Sub  // Google User ID
    );

    internal record GoogleTokens(
        string? IdToken,
        string? AccessToken
    );
}
