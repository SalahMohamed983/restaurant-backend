using System;

namespace ResturantBusinessLayer.Dtos.Reservations
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public Guid? TableId { get; set; }
        public DateTime? ReservationStart { get; set; }
        public DateTime? ReservationEnd { get; set; }
        public int GuestsCount { get; set; }
        public int Status { get; set; }
        public string? Notes { get; set; }
    }
}
