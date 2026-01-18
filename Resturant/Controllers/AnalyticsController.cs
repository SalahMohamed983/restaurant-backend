using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Resturant.Attributes;
using ResturantBusinessLayer.Services.Interfaces;
using System.Threading.Tasks;

namespace Resturant.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        [HttpGet("daily-kpis")]
        public async Task<IActionResult> GetDailyKpis()
        {
            var kpis = await _analyticsService.GetDailyKpisAsync();
            return Ok(kpis);
        }

        [HttpGet("monthly-kpis")]
        public async Task<IActionResult> GetMonthlyKpis()
        {
            var kpis = await _analyticsService.GetMonthlyKpisAsync();
            return Ok(kpis);
        }

        [HttpGet("top-items-daily")]
        public async Task<IActionResult> GetTopItemsDaily()
        {
            var items = await _analyticsService.GetTopItemsDailyAsync();
            return Ok(items);
        }

        [HttpGet("category-revenue-daily")]
        public async Task<IActionResult> GetCategoryRevenueDaily()
        {
            var revenue = await _analyticsService.GetCategoryRevenueDailyAsync();
            return Ok(revenue);
        }

        [HttpGet("reservations-by-hour-daily")]
        public async Task<IActionResult> GetReservationsByHourDaily()
        {
            var reservations = await _analyticsService.GetReservationsByHourDailyAsync();
            return Ok(reservations);
        }

        [HttpGet("table-utilization-daily")]
        public async Task<IActionResult> GetTableUtilizationDaily()
        {
            var utilization = await _analyticsService.GetTableUtilizationDailyAsync();
            return Ok(utilization);
        }
    }
}
