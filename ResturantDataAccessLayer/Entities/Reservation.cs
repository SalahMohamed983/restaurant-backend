using System;

namespace ResturantDataAccessLayer.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? TableId { get; set; }
        public DateTime? ReservationStart { get; set; }
        public DateTime? ReservationEnd { get; set; }
        public int GuestsCount { get; set; }
        public ReservationStatus Status { get; set; }
        public DateTime? RequestedAt { get; set; }
        public Guid? ApprovedByUserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? RejectReason { get; set; }
        public string? Notes { get; set; }

        // Navigation
        public User? Customer { get; set; }
        public DiningTable? Table { get; set; }
    }
}
