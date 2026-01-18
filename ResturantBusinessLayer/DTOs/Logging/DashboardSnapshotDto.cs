using System;

namespace ResturantBusinessLayer.Dtos.Logging
{
    public class DashboardSnapshotDto
    {
        public Guid Id { get; set; }
        public string? SnapshotKey { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? DataJson { get; set; }
        public DateTime? GeneratedAt { get; set; }
    }
}
