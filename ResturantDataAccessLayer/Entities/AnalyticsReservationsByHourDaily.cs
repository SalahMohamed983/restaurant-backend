using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsReservationsByHourDaily
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public int Hour { get; set; }
        public int ReservationsCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public int NoShowCount { get; set; }
    }
}
