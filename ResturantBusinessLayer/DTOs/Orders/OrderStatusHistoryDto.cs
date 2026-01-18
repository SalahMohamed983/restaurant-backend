using System;

namespace ResturantBusinessLayer.Dtos.Orders
{
    public class OrderStatusHistoryDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public int OldStatus { get; set; }
        public int NewStatus { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string? Comment { get; set; }
    }
}
