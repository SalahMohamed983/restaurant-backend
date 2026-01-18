using System;

namespace ResturantBusinessLayer.Dtos.Analytics
{
    public class AnalyticsTableUtilizationDailyDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public Guid TableId { get; set; }
        public int ReservationsCount { get; set; }
        public int ApprovedReservationsCount { get; set; }
        public int MinutesReserved { get; set; }
        public decimal UtilizationPercent { get; set; }
    }
}
