using System;
using System.Collections.Generic;

namespace ResturantBusinessLayer.Dtos.Orders
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ReservationId { get; set; }
        public int OrderType { get; set; }
        public int Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public DateTime? CreatedAt { get; set; }

        // Include order items
        public ICollection<OrderItemDto>? OrderItems { get; set; }
    }
}
