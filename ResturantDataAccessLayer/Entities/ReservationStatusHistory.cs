using System;

namespace ResturantDataAccessLayer.Entities
{
    public class ReservationStatusHistory
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public ReservationStatus OldStatus { get; set; }
        public ReservationStatus NewStatus { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string? Comment { get; set; }

        // Navigation
        public Reservation? Reservation { get; set; }
    }
}
