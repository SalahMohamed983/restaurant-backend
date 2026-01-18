using System;

namespace ResturantBusinessLayer.Dtos.Logging
{
    public class TransactionLogDto
    {
        public Guid Id { get; set; }
        public string? TransactionType { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public Guid? PerformedByUserId { get; set; }
        public DateTime? OccurredAt { get; set; }
    }
}
