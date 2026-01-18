using System;

namespace ResturantBusinessLayer.Dtos.Logging
{
    public class AuditLogDto
    {
        public Guid Id { get; set; }
        public Guid? ActorUserId { get; set; }
        public string? Action { get; set; }
        public string? EntityType { get; set; }
        public Guid? EntityId { get; set; }
        public string? OldValuesJson { get; set; }
        public string? NewValuesJson { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
