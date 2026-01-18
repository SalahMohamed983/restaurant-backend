using System;

namespace ResturantBusinessLayer.Dtos.Analytics
{
    public class AnalyticsMonthlyKpiDto
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int OrdersCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
