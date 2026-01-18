using System.ComponentModel.DataAnnotations;

namespace ResturantBusinessLayer.Dtos.Auth
{
    public class LogoutDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
