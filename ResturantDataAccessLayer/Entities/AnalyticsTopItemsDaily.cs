using System;

namespace ResturantDataAccessLayer.Entities
{
    public class AnalyticsTopItemsDaily
    {
        public Guid Id { get; set; }
        public DateOnly Date { get; set; }
        public Guid MenuItemId { get; set; }
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }
}
