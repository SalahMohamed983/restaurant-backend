using System.ComponentModel.DataAnnotations;

namespace ResturantBusinessLayer.Dtos.Auth
{
    public class ExternalLoginDto
    {
        [Required(ErrorMessage = "ID token is required")]
        public string IdToken { get; set; } = string.Empty;

        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = string.Empty; // "Google"
    }

    public class ExternalLoginCallbackDto
    {
        [Required(ErrorMessage = "Provider is required")]
        public string Provider { get; set; } = string.Empty;

        public string? Code { get; set; }
        public string? State { get; set; }
        public string? Error { get; set; }
    }
}
