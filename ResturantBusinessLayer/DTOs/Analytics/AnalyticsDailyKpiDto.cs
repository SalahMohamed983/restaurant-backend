using System;

namespace ResturantBusinessLayer.Dtos.Analytics
{
    public class AnalyticsDailyKpiDto
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public int OrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public int CancelledOrdersCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal TaxTotal { get; set; }
        public decimal DiscountTotal { get; set; }
    }
}
