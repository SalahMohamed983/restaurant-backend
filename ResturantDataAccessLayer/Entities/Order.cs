using System;
using System.Collections.Generic;

namespace ResturantDataAccessLayer.Entities
{
    public class Order
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? ReservationId { get; set; }
        public OrderType OrderType { get; set; }
        public OrderStatus Status { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal Discount { get; set; }
        public decimal Total { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public User? Customer { get; set; }
        public Reservation? Reservation { get; set; }
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}
