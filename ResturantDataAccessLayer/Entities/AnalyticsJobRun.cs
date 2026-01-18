using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsJobRun
    {
        public Guid Id { get; set; }
        public string? JobName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public AnalyticsJobStatus Status { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public string? Error { get; set; }
    }
}
