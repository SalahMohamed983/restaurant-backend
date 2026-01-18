using System;

namespace ResturantBusinessLayer.Dtos.Analytics
{
    public class AnalyticsReservationsByHourDailyDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public int Hour { get; set; }
        public int ReservationsCount { get; set; }
    }
}
