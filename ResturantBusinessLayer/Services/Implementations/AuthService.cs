using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ResturantBusinessLayer.Dtos.Auth;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantBusinessLayer.Settings;
using ResturantDataAccessLayer.Entities;
using ResturantDataAccessLayer.UnitOfWork;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _uow;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly HttpClient _httpClient;
        private readonly ILogger<AuthService> _logger;

        private readonly IEmailService _emailService;
        private readonly EmailSettings _emailSettings;
        private readonly IUserPermissionService _userPermissionService;

        public AuthService(
            UserManager<User> userManager,
            IUnitOfWork uow,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            IEmailService emailService,
            IOptions<EmailSettings> emailSettings,
            ILogger<AuthService> logger,
            IUserPermissionService userPermissionService)
        {
            _userManager = userManager;
            _uow = uow;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient();
            _emailService = emailService;
            _emailSettings = emailSettings.Value;
            _logger = logger;
            _userPermissionService = userPermissionService;
        }

        public async Task<bool> RegisterAsync(RegisterDto registerDto)
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                return false; // User already exists
            }

            // Create new user
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                PhoneNumber = registerDto.PhoneNumber,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                TokenVersion = 0,
                EmailConfirmed = false // Email not confirmed yet
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return false;
            }

            // Assign default role (you can modify this as needed)
            await _userManager.AddToRoleAsync(user, "User");

            // Generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode the token to make it URL-safe
            var encodedToken = Uri.EscapeDataString(confirmationToken);
            var baseUrl = _emailSettings.BaseUrl;
            var confirmationLink = $"{baseUrl}/?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            return true;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, string? ipAddress, string? userAgent)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || user.IsDeleted)
            {
                return null;
            }

            // Check if email is confirmed
            if (!user.EmailConfirmed)
            {
                return null; // Email not confirmed
            }

            var isValidPassword = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isValidPassword)
            {
                return null;
            }

            // Generate tokens
            var roles = await _userManager.GetRolesAsync(user);
            var token = await GenerateJwtTokenAsync(user, roles);

            // Extract JwtId from token to link refresh token
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(token);
            var jwtId = jwtSecurityToken.Id;

            var refreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress, userAgent, jwtId);

            return new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"] ?? "60")),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                }
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenDto, string? ipAddress, string? userAgent)
        {
            var principal = GetPrincipalFromExpiredToken(refreshTokenDto.Token);
            if (principal == null)
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            var user = await _userManager.FindByIdAsync(userIdClaim);
            if (user == null || user.IsDeleted)
            {
                return null;
            }

            // Validate TokenVersion
            var tokenVersionClaim = principal.FindFirst("TokenVersion")?.Value;
            if (string.IsNullOrEmpty(tokenVersionClaim) ||
                !int.TryParse(tokenVersionClaim, out var tokenVersion) ||
                tokenVersion != user.TokenVersion)
            {
                return null; // Token invalidated (password changed or token revoked)
            }

            // Extract JWT ID (Jti) from the expired token
            var jtiClaim = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jtiClaim))
            {
                return null; // Invalid token - missing Jti claim
            }

            // Validate refresh token
            var tokenHash = HashToken(refreshTokenDto.RefreshToken);
            var userRefreshToken = await _uow.RefreshTokens.Query()
                .FirstOrDefaultAsync(rt => rt.UserId == userId &&
                                     rt.TokenHash == tokenHash &&
                                     rt.JwtId == jtiClaim && // Verify JWT ID matches
                                     rt.ExpiresOn > DateTime.UtcNow &&
                                     rt.RevokedOn == null);

            if (userRefreshToken == null)
            {
                return null;
            }

            // Generate new tokens first
            var roles = await _userManager.GetRolesAsync(user);
            var newToken = await GenerateJwtTokenAsync(user, roles);

            // Extract JwtId from token to link refresh token
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(newToken);
            var jwtId = jwtSecurityToken.Id;

            // Generate new refresh token (but don't save yet - we need its ID first)
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var newRefreshTokenString = Convert.ToBase64String(randomNumber);

            var newRefreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                TokenHash = HashToken(newRefreshTokenString),
                JwtId = jwtId,
                ExpiresOn = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")),
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            // Add new refresh token (not saved yet)
            await _uow.RefreshTokens.AddAsync(newRefreshTokenEntity);

            // Revoke old refresh token and link it to the new one
            userRefreshToken.RevokedOn = DateTime.UtcNow;
            userRefreshToken.RevokedByIp = ipAddress;
            userRefreshToken.ReplacedByTokenId = newRefreshTokenEntity.Id;
            _uow.RefreshTokens.Update(userRefreshToken);

            // Save all changes (old token update + new token creation)
            await _uow.SaveChangesAsync();

            return new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshTokenString,
                ExpiresAt = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"] ?? "60")),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                }
            };
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
            if (result.Succeeded)
            {
                // Increment token version to invalidate all existing tokens
                user.TokenVersion++;
                await _userManager.UpdateAsync(user);
                // Revoke all refresh tokens
                var userTokens = await _uow.RefreshTokens.Query()
                    .Where(rt => rt.UserId == userId && rt.RevokedOn == null)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                    token.RevokedByIp = "Password Changed";
                }

                if (userTokens.Any())
                {
                    _uow.RefreshTokens.UpdateRange(userTokens);
                    await _uow.SaveChangesAsync();
                }
            }

            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || user.IsDeleted)
            {
                // Return true even if user doesn't exist (security best practice)
                return true;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // Encode the token to make it URL-safe
            var encodedToken = Uri.EscapeDataString(token);
            var baseUrl = _emailSettings.BaseUrl;
            var resetLink = $"{baseUrl}?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

            // Send password reset email
            await _emailService.SendPasswordResetAsync(user.Email!, resetLink);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
            if (user == null || user.IsDeleted)
            {
                _logger.LogWarning("Password reset attempt for non-existent or deleted user: {Email}", resetPasswordDto.Email);
                return false;
            }

            // Decode the token if it's URL-encoded (from email link copied to POST body)
            string decodedToken = resetPasswordDto.Token;
            try
            {
                // Try to decode if it contains URL encoding characters
                if (decodedToken.Contains("%"))
                {
                    decodedToken = Uri.UnescapeDataString(decodedToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decode URL-encoded token for password reset. Email: {Email}", resetPasswordDto.Email);
                // If decoding fails, use original token
                decodedToken = resetPasswordDto.Token;
            }

            var result = await _userManager.ResetPasswordAsync(user, decodedToken, resetPasswordDto.NewPassword);
            if (result.Succeeded)
            {
                // Increment token version to invalidate all existing tokens
                user.TokenVersion++;
                await _userManager.UpdateAsync(user);

                // Revoke all refresh tokens
                var userTokens = await _uow.RefreshTokens.Query()
                    .Where(rt => rt.UserId == user.Id && rt.RevokedOn == null)
                    .ToListAsync();

                foreach (var token in userTokens)
                {
                    token.RevokedOn = DateTime.UtcNow;
                    token.RevokedByIp = "Password Reset";
                }

                if (userTokens.Any())
                {
                    _uow.RefreshTokens.UpdateRange(userTokens);
                    await _uow.SaveChangesAsync();
                }

                _logger.LogInformation("Password reset successful for user: {Email}", resetPasswordDto.Email);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for user {Email}. Errors: {Errors}", resetPasswordDto.Email, errors);
            }

            return result.Succeeded;
        }

        public async Task<bool> LogoutAsync(string refreshToken, string? ipAddress)
        {
            var tokenHash = HashToken(refreshToken);
            var token = await _uow.RefreshTokens.Query()
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash && rt.RevokedOn == null);

            if (token == null)
            {
                return false;
            }

            // Find the latest active refresh token for this user (if exists) to link as replacement
            var latestActiveToken = await _uow.RefreshTokens.Query()
                .Where(rt => rt.UserId == token.UserId && 
                            rt.RevokedOn == null && 
                            rt.Id != token.Id &&
                            rt.ExpiresOn > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedOn)
                .FirstOrDefaultAsync();

            token.RevokedOn = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            // If there's a newer active token, link this revoked token to it
            if (latestActiveToken != null)
            {
                token.ReplacedByTokenId = latestActiveToken.Id;
            }
            _uow.RefreshTokens.Update(token);
            await _uow.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResponseDto?> GoogleLoginAsync(
                 GoogleLoginDto dto,
                 string? ipAddress,
                 string? userAgent)
        {
            using var transaction = await _uow.BeginTransactionAsync();
            try
            {
                // 1?? ????? Authorization Code ?? Tokens
                _logger.LogInformation("??? ????? Google Code ????????");
                var tokens = await ExchangeGoogleCodeAsync(dto.Code, dto.RedirectUri);

                if (tokens?.IdToken == null)
                {
                    _logger.LogWarning("??? ?????? ??? Google ID Token");
                    return null;
                }

                // 2?? ?????? ?? ID Token ??????? ??? ?????? ????????
                var userInfo = await ValidateGoogleIdTokenAsync(tokens.IdToken);

                if (userInfo == null || !userInfo.EmailVerified)
                {
                    _logger.LogWarning("Google email ??? ???? ?? token ??? ????");
                    return null;
                }

                // 3?? ????? ?? ???????? ?? ????? ???? ????
                var user = await _userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    // ????? ?????? ????
                    user = await CreateUserFromGoogleAsync(userInfo);
                    if (user == null)
                    {
                        await transaction.RollbackAsync();
                        return null;
                    }
                }
                else
                {
                    // ???????? ????? - ?????? ?? ?????
                    if (user.IsDeleted)
                    {
                        _logger.LogWarning("?????? ????? ???? ????? ?????: {Email}", userInfo.Email);
                        await transaction.RollbackAsync();
                        return null;
                    }

                    // ????? ???????? ?? Google
                    await UpdateUserFromGoogleAsync(user, userInfo);
                }

                // 4?? ??? External Login
                await LinkGoogleAccountAsync(user, userInfo.Sub);

                // 5?? ????? JWT ? Refresh Token
                var authResponse = await GenerateAuthTokensAsync(user, ipAddress, userAgent);

                await transaction.CommitAsync();

                _logger.LogInformation("?? ????? ???? Google ?????: {Email}", userInfo.Email);
                return authResponse;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "??? ?? ????? ?????? ?? Google");
                return null;
            }
        }



        /// <summary>
        /// ????? Google Authorization Code ?? Tokens
        /// </summary>
        private async Task<GoogleTokens?> ExchangeGoogleCodeAsync(string code, string redirectUri)
        {
            try
            {
                var clientId = _configuration["OAuth:Google:ClientId"];
                var clientSecret = _configuration["OAuth:Google:ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    _logger.LogError("Google OAuth credentials ??? ?????? ?? Configuration");
                    return null;
                }

                var tokenRequest = new Dictionary<string, string>
                {
                    ["code"] = code,
                    ["client_id"] = clientId,
                    ["client_secret"] = clientSecret,
                    ["redirect_uri"] = redirectUri,
                    ["grant_type"] = "authorization_code"
                };

                var response = await _httpClient.PostAsync(
                    "https://oauth2.googleapis.com/token",
                    new FormUrlEncodedContent(tokenRequest));

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogError("??? ????? Google Code. Status: {Status}, Response: {Error}",
                        response.StatusCode, error);
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var tokenResponse = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

                if (tokenResponse == null)
                {
                    return null;
                }

                var idToken = tokenResponse.ContainsKey("id_token")
                    ? tokenResponse["id_token"].GetString()
                    : null;

                var accessToken = tokenResponse.ContainsKey("access_token")
                    ? tokenResponse["access_token"].GetString()
                    : null;

                return new GoogleTokens(idToken, accessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception ?? ????? Google Authorization Code");
                return null;
            }
        }

        /// <summary>
        /// ?????? ?? Google ID Token
        /// </summary>
        private async Task<GoogleUserInfo?> ValidateGoogleIdTokenAsync(string idToken)
        {
            try
            {
                var clientId = _configuration["OAuth:Google:ClientId"];
                if (string.IsNullOrEmpty(clientId))
                {
                    return null;
                }

                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { clientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                if (payload?.Email == null)
                {
                    return null;
                }

                return new GoogleUserInfo(
                    Email: payload.Email,
                    EmailVerified: payload.EmailVerified,
                    Name: payload.Name,
                    Picture: payload.Picture,
                    Sub: payload.Subject
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "??? ?????? ?? Google ID Token");
                return null;
            }
        }

        /// <summary>
        /// ????? ?????? ???? ?? Google
        /// </summary>
        private async Task<User?> CreateUserFromGoogleAsync(GoogleUserInfo userInfo)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = userInfo.Email,
                Email = userInfo.Email,
                EmailConfirmed = true, // Google ??? ??????
                FullName = userInfo.Name ?? userInfo.Email.Split('@')[0],
                ImageUser = userInfo.Picture,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false,
                TokenVersion = 0
            };

            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("??? ????? ?????? Google: {Errors}", errors);
                return null;
            }

            // ????? ??? User
            await _userManager.AddToRoleAsync(user, "User");

            _logger.LogInformation("?? ????? ?????? ???? ?? Google: {Email}", userInfo.Email);
            return user;
        }

        /// <summary>
        /// ????? ?????? ???????? ?? Google
        /// </summary>
        private async Task UpdateUserFromGoogleAsync(User user, GoogleUserInfo userInfo)
        {
            bool updated = false;

            if (!string.IsNullOrEmpty(userInfo.Name) && user.FullName != userInfo.Name)
            {
                user.FullName = userInfo.Name;
                updated = true;
            }

            if (!string.IsNullOrEmpty(userInfo.Picture) && user.ImageUser != userInfo.Picture)
            {
                user.ImageUser = userInfo.Picture;
                updated = true;
            }

            if (updated)
            {
                await _userManager.UpdateAsync(user);
            }
        }

        /// <summary>
        /// ??? ???? Google ?????????
        /// </summary>
        private async Task LinkGoogleAccountAsync(User user, string googleId)
        {
            var logins = await _userManager.GetLoginsAsync(user);
            var hasGoogle = logins.Any(l => l.LoginProvider == "Google" && l.ProviderKey == googleId);

            if (!hasGoogle)
            {
                var loginInfo = new UserLoginInfo("Google", googleId, "Google");
                await _userManager.AddLoginAsync(user, loginInfo);
            }
        }
        private async Task<AuthResponseDto> GenerateAuthTokensAsync(
          User user,
          string? ipAddress,
          string? userAgent)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var jwtToken = await GenerateJwtTokenAsync(user, roles);

            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
            var jwtId = jwtSecurityToken.Id;

            var refreshToken = await GenerateRefreshTokenAsync(user.Id, ipAddress, userAgent, jwtId);

            return new AuthResponseDto
            {
                Token = jwtToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(
                    Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"] ?? "60")),
                User = new UserInfoDto
                {
                    Id = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Roles = roles
                }
            };
        }


        public async Task<bool> ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(confirmEmailDto.Email);
            if (user == null || user.IsDeleted)
            {
                return false;
            }

            if (user.EmailConfirmed)
            {
                return true; // Already confirmed
            }

            // Decode the token if it's URL-encoded (from email link)
            // Token from query string is auto-decoded, but from POST body it may be encoded
            string decodedToken = confirmEmailDto.Token;
            try
            {
                // Try to decode if it contains URL encoding characters
                if (decodedToken.Contains("%"))
                {
                    decodedToken = Uri.UnescapeDataString(decodedToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to decode URL-encoded token for email confirmation. Email: {Email}", confirmEmailDto.Email);
                // If decoding fails, use original token
                decodedToken = confirmEmailDto.Token;
            }

            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            // Log errors for debugging
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Email confirmation failed for {Email}. Errors: {Errors}", confirmEmailDto.Email, errors);
            }

            return result.Succeeded;
        }

        public async Task<bool> ResendConfirmationEmailAsync(ResendConfirmationEmailDto resendConfirmationEmailDto)
        {
            var user = await _userManager.FindByEmailAsync(resendConfirmationEmailDto.Email);
            if (user == null || user.IsDeleted)
            {
                // Return true even if user doesn't exist (security best practice)
                return true;
            }

            if (user.EmailConfirmed)
            {
                return true; // Email already confirmed
            }

            // Generate email confirmation token
            var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            // Encode the token to make it URL-safe
            var encodedToken = Uri.EscapeDataString(confirmationToken);
            var baseUrl = _emailSettings.BaseUrl;

            var confirmationLink = $"{baseUrl}?email={Uri.EscapeDataString(user.Email!)}&token={encodedToken}";

            // Send confirmation email
            await _emailService.SendEmailConfirmationAsync(user.Email!, confirmationLink);

            return true;
        }



     

        /// ////////////////////////////////

        private async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim("FullName", user.FullName ?? string.Empty),
                new Claim("TokenVersion", user.TokenVersion.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Add permissions to claims
            var permissions = await _userPermissionService.GetUserPermissionCodesAsync(user.Id);
            foreach (var permission in permissions)
            {
                claims.Add(new Claim("Permission", permission));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationMinutes"] ?? "60")),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateRefreshTokenAsync(Guid userId, string? ipAddress, string? userAgent, string jwtId)
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            var refreshToken = Convert.ToBase64String(randomNumber);

            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TokenHash = HashToken(refreshToken),
                JwtId = jwtId, // Link to JWT token's Jti claim
                ExpiresOn = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7")),
                CreatedOn = DateTime.UtcNow,
                CreatedByIp = ipAddress,
                UserAgent = userAgent
            };

            await _uow.RefreshTokens.AddAsync(refreshTokenEntity);
            await _uow.SaveChangesAsync();

            return refreshToken;
        }

        private string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
        /// ////////////////////////////////
        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured"))),
                ValidateLifetime = false // We want to validate expired tokens
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get principal from expired JWT token");
                return null;
            }
        }


    } }
