using Riok.Mapperly.Abstractions;
using ResturantBusinessLayer.Dtos.Users;
using ResturantBusinessLayer.Dtos.Menu.Categories;
using ResturantBusinessLayer.Dtos.Menu.Items;
using ResturantBusinessLayer.Dtos.Reservations;
using ResturantBusinessLayer.Dtos.Reservations.Tables;
using ResturantBusinessLayer.Dtos.Orders;
using ResturantBusinessLayer.Dtos.Payments;
using ResturantBusinessLayer.Dtos.Analytics;
using ResturantDataAccessLayer.Entities;

namespace ResturantBusinessLayer.Mappers
{
    [Mapper]
    public partial class EntityMappers
    {
        // User
        public partial UserDto Map(User source);
        public partial User Map(UserDto source);

        // Role
        public partial RoleDto Map(AspNetRole source);
        public partial AspNetRole Map(RoleDto source);

        // Permission
        public partial PermissionDto Map(Permission source);
        public partial Permission Map(PermissionDto source);

        // RolePermission
        public partial RolePermissionDto Map(RolePermission source);
        public partial RolePermission Map(RolePermissionDto source);

        // Categories
        public partial MenuCategoryDto Map(MenuCategory source);
        public partial MenuCategory Map(MenuCategoryDto source);
        // Menu
        public partial MenuItemDto Map(MenuItem source);
        public partial MenuItem Map(MenuItemDto source);

        // Reservation
        public partial ReservationDto Map(Reservation source);
        public partial Reservation Map(ReservationDto source);
        public partial DiningTableDto Map(DiningTable source);
        public partial DiningTable Map(DiningTableDto source);

        // Order
        public partial OrderDto Map(Order source);
        public partial Order Map(OrderDto source);
        public partial OrderItemDto Map(OrderItem source);
        public partial OrderItem Map(OrderItemDto source);

        // Payment
        public partial PaymentDto Map(Payment source);
        public partial Payment Map(PaymentDto source);

        // Analytics
        public partial AnalyticsDailyKpiDto Map(AnalyticsDailyKpi source);
        public partial AnalyticsDailyKpi Map(AnalyticsDailyKpiDto source);
        public partial AnalyticsMonthlyKpiDto Map(AnalyticsMonthlyKpi source);
        public partial AnalyticsMonthlyKpi Map(AnalyticsMonthlyKpiDto source);
        public partial AnalyticsTopItemsDailyDto Map(AnalyticsTopItemsDaily source);
        public partial AnalyticsTopItemsDaily Map(AnalyticsTopItemsDailyDto source);
        public partial AnalyticsCategoryRevenueDailyDto Map(AnalyticsCategoryRevenueDaily source);
        public partial AnalyticsCategoryRevenueDaily Map(AnalyticsCategoryRevenueDailyDto source);
        public partial AnalyticsReservationsByHourDailyDto Map(AnalyticsReservationsByHourDaily source);
        public partial AnalyticsReservationsByHourDaily Map(AnalyticsReservationsByHourDailyDto source);
        public partial AnalyticsTableUtilizationDailyDto Map(AnalyticsTableUtilizationDaily source);
        public partial AnalyticsTableUtilizationDaily Map(AnalyticsTableUtilizationDailyDto source);
    }
}
