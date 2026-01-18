using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsMonthlyKpi
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int OrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public decimal Revenue { get; set; }
        public int ReservationsCount { get; set; }
        public int ApprovedReservationsCount { get; set; }
        public int RejectedReservationsCount { get; set; }
        public int UniqueCustomersCount { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
