using System;

namespace ResturantDataAccessLayer.Entities
{
    public class TransactionLog
    {
        public Guid Id { get; set; }
        public string? TransactionType { get; set; }
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public decimal Amount { get; set; }
        public Guid? PerformedByUserId { get; set; }
        public DateTime? OccurredAt { get; set; }
        public string? MetaJson { get; set; }

        // Navigation
        public User? PerformedBy { get; set; }
    }
}
