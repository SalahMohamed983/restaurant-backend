using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ResturantBusinessLayer.Dtos.Analytics;

namespace ResturantBusinessLayer.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<IEnumerable<AnalyticsDailyKpiDto>> GetDailyKpisAsync();
        Task<IEnumerable<AnalyticsMonthlyKpiDto>> GetMonthlyKpisAsync();
        Task<IEnumerable<AnalyticsTopItemsDailyDto>> GetTopItemsDailyAsync();
        Task<IEnumerable<AnalyticsCategoryRevenueDailyDto>> GetCategoryRevenueDailyAsync();
        Task<IEnumerable<AnalyticsReservationsByHourDailyDto>> GetReservationsByHourDailyAsync();
        Task<IEnumerable<AnalyticsTableUtilizationDailyDto>> GetTableUtilizationDailyAsync();
    }
}
