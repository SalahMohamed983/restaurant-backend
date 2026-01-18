using System;

namespace ResturantDataAccessLayer.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }
        public string? ProviderRef { get; set; }
        public DateTime? PaidAt { get; set; }
        public DateTime? CreatedAt { get; set; }
        public Guid? CreatedByUserId { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }
}
