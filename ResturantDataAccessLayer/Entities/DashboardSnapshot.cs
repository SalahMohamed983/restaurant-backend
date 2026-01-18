using System;

namespace ResturantDataAccessLayer.Entities
{
    public class DashboardSnapshot
    {
        public Guid Id { get; set; }
        public string? SnapshotKey { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? DataJson { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public string? GeneratedBy { get; set; }
    }
}
