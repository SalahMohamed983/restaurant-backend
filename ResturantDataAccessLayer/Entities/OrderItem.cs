using System;

namespace ResturantDataAccessLayer.Entities
{
    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid MenuItemId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public Order? Order { get; set; }
        public MenuItem? MenuItem { get; set; }
    }
}
