using System;

namespace ResturantDataAccessLayer.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string? TokenHash { get; set; }
        public string? JwtId { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public Guid? ReplacedByTokenId { get; set; }
        public string? CreatedByIp { get; set; }
        public string? RevokedByIp { get; set; }
        public string? UserAgent { get; set; }

        // Navigation
        public User? User { get; set; }
    }
}
