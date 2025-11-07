
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesStock.Domain.Entities;
using SalesStock.Domain.Enums;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Web.ViewModels.Stock;
using SalesStock.Web.Common;


namespace YourProjectName.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class StockController : Controller
    {
        private readonly AppDbContext _context;

        public StockController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.StockMovements.AsNoTracking().Include(sm => sm.Product).OrderByDescending(sm => sm.CreatedAt);

            int pageSize = 20;
            var pagedList = await PaginatedList<StockMovement>.CreateAsync(query, 1, pageSize);
            return View(pagedList);
        }

        public IActionResult Adjust()
        {
            return View(new StockAdjustViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjustViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.SKU == model.SKU);

            if (product == null)
            {
                ModelState.AddModelError("Sku", "Product with this SKU not found.");
                return View(model);
            }
            int quantityAsInt = (int)model.Quantity;

            if (quantityAsInt != model.Quantity)
            {
                ModelState.AddModelError("Quantity", "Stock quantity must be a whole number.");
                return View(model);
            }

            if (product.StockOnHand + quantityAsInt < 0)
            {
                ModelState.AddModelError("Quantity", $"Operation failed. This adjustment would result in negative stock ({product.StockOnHand + model.Quantity}). Current stock is {product.StockOnHand}.");
                return View(model);
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    int oldStock = (int)product.StockOnHand;
                    product.StockOnHand += model.Quantity;
                    _context.Update(product);

                    var stockMovement = new StockMovement
                    {
                        ProductId = product.Id,
                        Quantity = model.Quantity,
                        MovementType = StockMovementType.Adjust,
                        ReferenceNo = $"ADJ-{System.DateTime.UtcNow:yyyyMMddHHmmss}"
                    };
                    _context.StockMovements.Add(stockMovement);

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    model.ProductName = product.Name;
                    model.OldStock = oldStock;
                    model.NewStock = (int)product.StockOnHand;

                    TempData["SuccessMessage"] = $"Stock for '{product.Name}' adjusted successfully from {oldStock} to {product.StockOnHand}.";

                    return View("AdjustResult", model);
                }
                catch (System.Exception)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = "An unexpected error occurred. The transaction has been rolled back.";
                    return RedirectToAction(nameof(Adjust));
                }
            }
        }
    }
}