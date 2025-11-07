using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Domain.Entities;
using SalesStock.Web.Common;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class ProductsController : Controller
    {
       private readonly AppDbContext _context;
         public ProductsController(AppDbContext context)
         {
              _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder,string currentFilter,string searchTerm,string statusFilter,int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["SkuSortParam"] = string.IsNullOrEmpty(sortOrder) ? "sku_desc" : "";
            ViewData["NameSortParam"] = sortOrder == "Name" ? "name_desc" : "Name";

            if (searchTerm != null || statusFilter != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchTerm = currentFilter;
            }

            ViewData["CurrentFilter"] = searchTerm;
            ViewData["CurrentStatusFilter"] = statusFilter;

            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.SKU.Contains(searchTerm)
                                       || p.Name.Contains(searchTerm)
                                       || (p.BarCode != null && p.BarCode.Contains(searchTerm)));
            }

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (statusFilter.Equals("active", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => p.IsActive);
                }
                else if (statusFilter.Equals("passive", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(p => !p.IsActive);
                }
            }

            switch (sortOrder)
            {
                case "sku_desc":
                    query = query.OrderByDescending(p => p.SKU);
                    break;
                case "Name":
                    query = query.OrderBy(p => p.Name);
                    break;
                case "name_desc":
                    query = query.OrderByDescending(p => p.Name);
                    break;
                default:
                    query = query.OrderBy(p => p.SKU);
                    break;
            }

            int pageSize = 10;
            int currentPage = pageNumber ?? 1;
            var pagedList = await PaginatedList<Product>.CreateAsync(query.AsNoTracking(), currentPage, pageSize);

            return View(pagedList);
        }


        public IActionResult Create()
        {
            var product = new Product
            {
                VatRate = 0.2m,
                IsActive = true
            };
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SKU,Name,Unit,VatRate,UnitPrice,BarCode,IsActive")] Product product)
        {
            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU))
            {
                ModelState.AddModelError("Sku", "This SKU already exists.");
            }
            if (product.UnitPrice <= 0)
            {
                ModelState.AddModelError("UnitPrice", "Unit Price must be greater than 0.");
            }
            if (product.VatRate < 0 || product.VatRate > 1)
            {
                ModelState.AddModelError("VatRate", "VAT Rate must be between 0 and 1 (e.g., 0.18 for 18%).");
            }

            if (ModelState.IsValid)
            {
                product.StockOnHand = 0;
                product.StockReserved = 0;

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Product '{product.Name}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            if (await _context.Products.AnyAsync(p => p.SKU == product.SKU && p.Id != product.Id))
            {
                ModelState.AddModelError("Sku", "This SKU is already used by another product.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(p => p.Id == id)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = $"Product '{product.Name}' was updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            product.IsActive = !product.IsActive;
            _context.Update(product);
            await _context.SaveChangesAsync();

            var status = product.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"Product '{product.Name}' has been {status}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
