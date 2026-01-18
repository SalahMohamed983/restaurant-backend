using System;

namespace ResturantBusinessLayer.Dtos.Payments
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public int Method { get; set; }
        public int Status { get; set; }
        public string? ProviderRef { get; set; }
        public DateTime? PaidAt { get; set; }
    }
}
