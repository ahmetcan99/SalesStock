using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SalesStock.Application.Features.Dashboard.DTOs;
using SalesStock.Application.Interfaces;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] DashboardFilterDTO filter)
        {
            if (!Enum.IsDefined(typeof(DashboardQuickRange), filter.Range))
            {
                filter.Range = DashboardQuickRange.Today;
            }
            var result = await _dashboardService.GetAsync(filter);
            return View(result);
        }
    }
}
