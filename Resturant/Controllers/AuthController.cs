using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ResturantBusinessLayer.Dtos.Auth;
using ResturantBusinessLayer.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
         IAuthService authService,
         IConfiguration configuration,
         ILogger<AuthController> logger)
        {
            _authService = authService;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerDto);
            if (!result)
            {
                return BadRequest(new { message = "Registration failed. User already exist." });
            }

            return Ok(new { message = "Registration successful. Please check your email to confirm your account." });
        }
       
        [EnableRateLimiting("auth")]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.LoginAsync(loginDto, ipAddress, userAgent);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid email or password, or email not confirmed." });
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.RefreshTokenAsync(refreshTokenDto, ipAddress, userAgent);
            if (result == null)
            {
                return Unauthorized(new { message = "Invalid token or refresh token." });
            }

            return Ok(result);
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new { message = "Invalid user." });
            }

            var result = await _authService.ChangePasswordAsync(userId, changePasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to change password. Please check your current password." });
            }

            return Ok(new { message = "Password changed successfully. All your sessions have been logged out." });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            // Always return success for security reasons (don't reveal if email exists)
            return Ok(new { message = "If the email exists, a password reset link has been sent." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Decode token if it's URL-encoded (from email link copied to POST body)
            if (!string.IsNullOrEmpty(resetPasswordDto.Token) && resetPasswordDto.Token.Contains("%"))
            {
                try
                {
                    resetPasswordDto.Token = Uri.UnescapeDataString(resetPasswordDto.Token);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to decode URL-encoded token for password reset. Email: {Email}", resetPasswordDto.Email);
                    // If decoding fails, use original token
                }
            }

            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to reset password. Token may be invalid or expired." });
            }

            return Ok(new { message = "Password reset successfully." });
        }


        

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Decode token if it's URL-encoded (from email link copied to POST body)
            if (!string.IsNullOrEmpty(confirmEmailDto.Token) && confirmEmailDto.Token.Contains("%"))
            {
                    confirmEmailDto.Token = Uri.UnescapeDataString(confirmEmailDto.Token);
            }

            var result = await _authService.ConfirmEmailAsync(confirmEmailDto);
            if (!result)
            {
                return BadRequest(new { message = "Failed to confirm email. Token may be invalid or expired." });
            }

            return Ok(new { message = "Email confirmed successfully." });
        }



        [HttpPost("resend-confirmation-email")]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDto resendConfirmationEmailDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.ResendConfirmationEmailAsync(resendConfirmationEmailDto);
            // Always return success for security reasons (don't reveal if email exists)
            return Ok(new { message = "If the email exists and is not confirmed, a confirmation email has been sent." });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout([FromBody] LogoutDto logoutDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.LogoutAsync(logoutDto.RefreshToken, ipAddress);

            if (!result)
            {
                return BadRequest(new { message = "Failed to logout. Invalid refresh token." });
            }

            return Ok(new { message = "Logged out successfully." });
        }


        [HttpGet("google/url")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetGoogleLoginUrl([FromQuery] string? returnUrl = null)
        {
            var clientId = _configuration["OAuth:Google:ClientId"];
            if (string.IsNullOrEmpty(clientId))
            {
                return StatusCode(500, new { message = "Google OAuth غير مُعد" });
            }

            // الـ Redirect URI الثابت (نفس الموجود في Google Console)
            var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google/callback";

            // بناء URL
            var state = string.IsNullOrEmpty(returnUrl)
                ? Guid.NewGuid().ToString("N")
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(returnUrl));

            var queryParams = new Dictionary<string, string>
            {
                ["client_id"] = clientId,
                ["redirect_uri"] = redirectUri,
                ["response_type"] = "code",
                ["scope"] = "openid email profile",
                ["state"] = state,
                ["access_type"] = "offline",
                ["prompt"] = "select_account"
            };

            var query = string.Join("&", queryParams.Select(kvp =>
                $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var url = $"https://accounts.google.com/o/oauth2/v2/auth?{query}";

            return Ok(new
            {
                url,
                message = "افتح هذا الرابط لتسجيل الدخول بـ Google"
            });
        }

        /// <summary>
        /// معالجة Callback من Google (GET) - يُستدعى تلقائياً من Google
        /// </summary>
        [HttpGet("google/callback")]
        public async Task<IActionResult> GoogleCallback(
            [FromQuery] string? code,
            [FromQuery] string? state,
            [FromQuery] string? error)
        {
            // معالجة الأخطاء من Google
            if (!string.IsNullOrEmpty(error))
            {
                _logger.LogWarning("Google OAuth Error: {Error}", error);
                return Redirect(GetFrontendUrl($"/login?error={Uri.EscapeDataString(error)}"));
            }

            if (string.IsNullOrEmpty(code))
            {
                return Redirect(GetFrontendUrl("/login?error=no_code"));
            }

            try
            {
                // الـ Redirect URI (نفس الموجود في Google Console)
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google/callback";

                var dto = new GoogleLoginDto
                {
                    Code = code,
                    RedirectUri = redirectUri
                };

                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();

                var result = await _authService.GoogleLoginAsync(dto, ipAddress, userAgent);

                if (result == null)
                {
                    return Redirect(GetFrontendUrl("/login?error=auth_failed"));
                }

                // فك تشفير الـ State للحصول على returnUrl
                string returnUrl = "/";
                if (!string.IsNullOrEmpty(state))
                {
                    try
                    {
                        var decodedState = Encoding.UTF8.GetString(Convert.FromBase64String(state));
                        returnUrl = decodedState;
                    }
                    catch
                    {
                        // إذا فشل، استخدم الصفحة الافتراضية
                    }
                }

                // إعادة توجيه للفرونت إند مع التوكنات
                var successUrl = GetFrontendUrl($"/auth/success?token={Uri.EscapeDataString(result.Token)}" +
                    $"&refreshToken={Uri.EscapeDataString(result.RefreshToken)}" +
                    $"&returnUrl={Uri.EscapeDataString(returnUrl)}");

                return Redirect(successUrl);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة Google Callback");
                return Redirect(GetFrontendUrl("/login?error=server_error"));
            }
        }

        /// <summary>
        /// تسجيل الدخول بـ Google (POST) - للاستخدام من Frontend مباشرة
        /// </summary>
        [HttpPost("google/login")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var userAgent = Request.Headers["User-Agent"].ToString();

            var result = await _authService.GoogleLoginAsync(dto, ipAddress, userAgent);

            if (result == null)
            {
                return Unauthorized(new { message = "فشل تسجيل الدخول بـ Google" });
            }

            return Ok(result);
        }

        /// <summary>
        /// الحصول على Frontend URL من الإعدادات
        /// </summary>
        private string GetFrontendUrl(string path = "")
        {
            var baseUrl = _configuration["Frontend:BaseUrl"] ?? "http://localhost:5173";
            return $"{baseUrl.TrimEnd('/')}{path}";
        }
    }
}
