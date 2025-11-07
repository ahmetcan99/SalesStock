using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesStock.Domain.Entities;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Web.ViewModels.PriceLists;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager")]
    public class PriceListsController : Controller
    {
        private readonly AppDbContext _context;

        public PriceListsController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return View(await _context.PriceLists.OrderBy(pl => pl.Name).ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PriceList priceList)
        {
            if (ModelState.IsValid)
            {
                if (priceList.IsDefault)
                {
                    await _context.PriceLists.Where(pl => pl.IsDefault).ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));
                }

                _context.Add(priceList);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Price List '{priceList.Name}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(priceList);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var priceList = await _context.PriceLists.FindAsync(id);
            if (priceList == null) return NotFound();
            return View(priceList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PriceList priceList)
        {
            if (id != priceList.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (priceList.IsDefault)
                {
                    await _context.PriceLists.Where(pl => pl.IsDefault && pl.Id != priceList.Id)
                                              .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));
                }

                try
                {
                    _context.Update(priceList);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.PriceLists.Any(pl => pl.Id == id)) return NotFound();
                    else throw;
                }
                TempData["SuccessMessage"] = $"Price List '{priceList.Name}' was updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(priceList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> SetDefault(int id)
        {
            var priceListToSet = await _context.PriceLists.FindAsync(id);
            if (priceListToSet == null)
            {
                return NotFound();
            }

            if (priceListToSet.IsDefault)
            {
                TempData["InfoMessage"] = $"'{priceListToSet.Name}' is already the default price list.";
                return RedirectToAction(nameof(Index));
            }

            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.PriceLists
                    .Where(pl => pl.IsDefault)
                    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsDefault, false));

                priceListToSet.IsDefault = true;
                _context.Update(priceListToSet);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                TempData["SuccessMessage"] = $"'{priceListToSet.Name}' has been set as the default price list.";
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                TempData["ErrorMessage"] = "An error occurred while setting the default price list.";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ManageItems(int id)
        {
            var priceList = await _context.PriceLists
                .AsNoTracking()
                .FirstOrDefaultAsync(pl => pl.Id == id);

            if (priceList == null) return NotFound();

            var items = await _context.PriceListItems
                .AsNoTracking()
                .Include(pli => pli.Product)
                .Where(pli => pli.PriceListId == id)
                .Select(pli => new PriceListItemViewModel
                {
                    Id = pli.Id,
                    ProductId = pli.ProductId,
                    Sku = pli.Product.SKU,
                    ProductName = pli.Product.Name,
                    Price = pli.UnitPrice,
                    ValidFrom = pli.ValidFrom,
                    ValidTo = pli.ValidTo
                })
                .OrderBy(item => item.Sku).ThenBy(item => item.ValidFrom)
                .ToListAsync();

            var model = new ManagePriceListItemsViewModel
            {
                PriceListId = priceList.Id,
                PriceListName = priceList.Name,
                Items = items,
                NewItem = new AddPriceListItemViewModel { PriceListId = id }
            };

            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(AddPriceListItemViewModel newItem)
        {
            if (ModelState.IsValid)
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.SKU == newItem.Sku);

                if (product == null)
                {
                    TempData["ErrorMessage"] = $"Product with SKU '{newItem.Sku}' not found.";
                    return RedirectToAction(nameof(ManageItems), new { id = newItem.PriceListId });
                }
                var isOverlapping = await _context.PriceListItems
                    .AnyAsync(pli => pli.PriceListId == newItem.PriceListId &&
                                     pli.ProductId == product.Id &&
                                     newItem.ValidFrom <= pli.ValidTo &&
                                     newItem.ValidTo >= pli.ValidFrom);

                if (isOverlapping)
                {
                    TempData["ErrorMessage"] = "The provided date range overlaps with an existing price for this product.";
                    return RedirectToAction(nameof(ManageItems), new { id = newItem.PriceListId });
                }

                var priceListItem = new PriceListItem
                {
                    PriceListId = newItem.PriceListId,
                    ProductId = product.Id,
                    UnitPrice = newItem.Price,
                    ValidFrom = newItem.ValidFrom,
                    ValidTo = newItem.ValidTo
                };

                _context.PriceListItems.Add(priceListItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Price list item was added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid data provided. Please check the form.";
            }

            return RedirectToAction(nameof(ManageItems), new { id = newItem.PriceListId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int id, int priceListId)
        {
            var item = await _context.PriceListItems.FindAsync(id);
            if (item != null)
            {
                _context.PriceListItems.Remove(item);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Price list item was removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Item not found.";
            }

            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchAvailableProducts(string term)
        {
            var query = _context.Products
                .AsNoTracking()
                .Where(p => p.IsActive);

            if (!string.IsNullOrEmpty(term))
            {
                query = query.Where(p => p.SKU.Contains(term) || p.Name.Contains(term));
            }

            var products = await query
                .Select(p => new {
                    value = p.SKU,
                    text = $"{p.SKU} - {p.Name}"
                })
                .OrderBy(p => p.value)
                .Take(50)
                .ToListAsync();

            return Json(products);
        }
    }
}
