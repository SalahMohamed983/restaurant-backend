using ResturantBusinessLayer.Dtos.Auth;
using System;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, string? ipAddress, string? userAgent);
        Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto, string? ipAddress, string? userAgent);
        Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto);
        Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<bool> LogoutAsync(string refreshToken, string? ipAddress);
        Task<bool> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto);
        Task<bool> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto);
        Task<AuthResponseDto?> GoogleLoginAsync(
              GoogleLoginDto dto,
              string? ipAddress,
              string? userAgent);

    }
}
