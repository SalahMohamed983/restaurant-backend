using System;

namespace ResturantBusinessLayer.Dtos.Reservations
{
    public class ReservationStatusHistoryDto
    {
        public Guid Id { get; set; }
        public Guid ReservationId { get; set; }
        public int OldStatus { get; set; }
        public int NewStatus { get; set; }
        public Guid? ChangedByUserId { get; set; }
        public DateTime? ChangedAt { get; set; }
        public string? Comment { get; set; }
    }
}
