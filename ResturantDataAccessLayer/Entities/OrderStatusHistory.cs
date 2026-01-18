using System;

namespace ResturantDataAccessLayer.Entities
{
    public class OrderStatusHistory
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public OrderStatus OldStatus { get; set; }
        public OrderStatus NewStatus { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string? Comment { get; set; }

        // Navigation
        public Order? Order { get; set; }
    }
}
