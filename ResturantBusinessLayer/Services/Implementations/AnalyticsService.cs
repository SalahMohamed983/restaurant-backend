using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Analytics;
using ResturantBusinessLayer.Services.Interfaces;
using ResturantDataAccessLayer.UnitOfWork;

namespace ResturantBusinessLayer.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _uow;
        public AnalyticsService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<IEnumerable<AnalyticsDailyKpiDto>> GetDailyKpisAsync()
        {
            var items = await _uow.AnalyticsDailyKpis.GetAllAsync();
            return items.Select(a => new AnalyticsDailyKpiDto
            {
                Id = a.Id,
                Date = a.Date,
                OrdersCount = a.OrdersCount,
                CompletedOrdersCount = a.CompletedOrdersCount,
                CancelledOrdersCount = a.CancelledOrdersCount,
                Revenue = a.Revenue,
                TaxTotal = a.TaxTotal,
                DiscountTotal = a.DiscountTotal
            });
        }

        public async Task<IEnumerable<AnalyticsMonthlyKpiDto>> GetMonthlyKpisAsync()
        {
            var items = await _uow.AnalyticsMonthlyKpis.GetAllAsync();
            return items.Select(a => new AnalyticsMonthlyKpiDto
            {
                Id = a.Id,
                Year = a.Year,
                Month = a.Month,
                OrdersCount = a.OrdersCount,
                CompletedOrdersCount = a.CompletedOrdersCount,
                Revenue = a.Revenue
            });
        }

        public async Task<IEnumerable<AnalyticsTopItemsDailyDto>> GetTopItemsDailyAsync()
        {
            var items = await _uow.AnalyticsTopItemsDaily.GetAllAsync();
            return items.Select(a => new AnalyticsTopItemsDailyDto
            {
                Id = a.Id,
                Date = a.Date,
                MenuItemId = a.MenuItemId,
                QuantitySold = a.QuantitySold,
                Revenue = a.Revenue
            });
        }

        public async Task<IEnumerable<AnalyticsCategoryRevenueDailyDto>> GetCategoryRevenueDailyAsync()
        {
            var items = await _uow.AnalyticsCategoryRevenueDaily.GetAllAsync();
            return items.Select(a => new AnalyticsCategoryRevenueDailyDto
            {
                Id = a.Id,
                Date = a.Date,
                CategoryId = a.CategoryId,
                OrdersCount = a.OrdersCount,
                Revenue = a.Revenue
            });
        }

        public async Task<IEnumerable<AnalyticsReservationsByHourDailyDto>> GetReservationsByHourDailyAsync()
        {
            var items = await _uow.AnalyticsReservationsByHourDaily.GetAllAsync();
            return items.Select(a => new AnalyticsReservationsByHourDailyDto
            {
                Id = a.Id,
                Date = a.Date,
                Hour = a.Hour,
                ReservationsCount = a.ReservationsCount
            });
        }

        public async Task<IEnumerable<AnalyticsTableUtilizationDailyDto>> GetTableUtilizationDailyAsync()
        {
            var items = await _uow.AnalyticsTableUtilizationDaily.GetAllAsync();
            return items.Select(a => new AnalyticsTableUtilizationDailyDto
            {
                Id = a.Id,
                Date = a.Date,
                TableId = a.TableId,
                ReservationsCount = a.ReservationsCount,
                ApprovedReservationsCount = a.ApprovedReservationsCount,
                MinutesReserved = a.MinutesReserved,
                UtilizationPercent = a.UtilizationPercent
            });
        }
    }
}
