using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesStock.Application.Interfaces;
using SalesStock.Application.Features.Stock.DTOs;

namespace Sal.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class StockController : Controller
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 20;
            int currentPage = pageNumber ?? 1;

            var pagedList = await _stockService.GetStockMovementsPagedAsync(currentPage, pageSize);

            return View(pagedList);
        }

        public IActionResult Adjust()
        {
            return View(new AdjustStockDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(AdjustStockDTO adjustDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(adjustDTO);
            }

            try
            {
                var resultDTO = await _stockService.AdjustStockAsync(adjustDTO);
                TempData["SuccessMessage"] = 
                    $"Stock adjusted successfully. New stock for SKU '{resultDTO.SKU}': {resultDTO.AdjustedQuantity}.";
                return View("AdjustResult", resultDTO);
            }
            catch (KeyNotFoundException ex)
            {
                ModelState.AddModelError("ProductId", ex.Message);
                return View(adjustDTO);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(adjustDTO);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(adjustDTO);
            }
        }
    }
}