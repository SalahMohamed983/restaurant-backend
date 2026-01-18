using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsCategoryRevenueDaily
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public Guid CategoryId { get; set; }
        public int OrdersCount { get; set; }
        public decimal Revenue { get; set; }
    }
}
