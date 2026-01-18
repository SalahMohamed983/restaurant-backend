using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsDailyKpi
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public int OrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public int CancelledOrdersCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
        public int ReservationsCount { get; set; }
        public int ApprovedReservationsCount { get; set; }
        public int RejectedReservationsCount { get; set; }
        public int CancelledReservationsCount { get; set; }
        public int NoShowReservationsCount { get; set; }
        public int UniqueCustomersCount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? GeneratedBy { get; set; }
    }
}
