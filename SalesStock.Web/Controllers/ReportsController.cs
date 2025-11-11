using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesStock.Application.Features.Reports.DTOs;
using SalesStock.Application.Interfaces;


namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;
        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }
        [HttpGet]
        public async Task<IActionResult> Index(DateTime? date, int? threshold)
        {
            var reportDate = date ?? DateTime.Today;
            var stockThreshold = threshold ?? 5;

            var dailySales = await _reportService.GetDailySalesAsync(reportDate);
            var lowStock = await _reportService.GetLowStockAsync(stockThreshold);

            var vm = new ReportViewModel
            {
                Date = reportDate,
                Threshold = stockThreshold,
                DailySales = dailySales,
                LowStock = lowStock
            };

            return View(vm);
        }

        public sealed class ReportViewModel
        {
            public DateTime Date { get; set; }
            public int Threshold { get; set; }
            public SalesStock.Application.Features.Reports.DTOs.DailySalesReportDTO DailySales { get; set; }
                = new();
            public IReadOnlyList<SalesStock.Application.Features.Reports.DTOs.LowStockItemDTO> LowStock
            { get; set; } = new List<SalesStock.Application.Features.Reports.DTOs.LowStockItemDTO>();
        }
    }
}

