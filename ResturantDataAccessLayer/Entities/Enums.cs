using System;

namespace ResturantDataAccessLayer.Entities
{
    public enum ReservationStatus { Pending, Approved, Rejected, Cancelled, }
    public enum OrderType { DineIn, Takeaway, Delivery }
    public enum OrderStatus { Pending, Confirmed, Preparing, Ready, Completed, Cancelled }
    public enum PaymentMethod { Cash, Card, Wallet, Other }
    public enum PaymentStatus { Pending, Paid, Failed, Refunded }
    public enum AnalyticsJobStatus { Success, Failed, Running }
}
