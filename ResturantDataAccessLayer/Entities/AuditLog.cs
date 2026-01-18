using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AuditLog
    {
        public Guid Id { get; set; }
        public Guid? ActorUserId { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Navigation
        public User? ActorUser { get; set; }
    }
}
