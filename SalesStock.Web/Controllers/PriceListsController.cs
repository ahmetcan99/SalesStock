using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesStock.Application.Interfaces;
using SalesStock.Application.Features.PriceLists.DTOs;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class PriceListsController : Controller
    {
        private readonly IPriceListService _priceListService;

        public PriceListsController(IPriceListService service)
        {
            _priceListService = service;
        }
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Index(int? pageNumber)
        {
            int pageSize = 10;
            var pagedList = await _priceListService.GetPriceListsPagedAsync(pageNumber ?? 1, pageSize);
            return View(pagedList);
        }
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }
        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePriceListDTO createPriceListDTO)
        {
            if (ModelState.IsValid)
            {
                await _priceListService.CreatePriceListAsync(createPriceListDTO);
                TempData["SuccessMessage"] = $"Price List '{createPriceListDTO.Name}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(createPriceListDTO);
        }
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var priceListDTO = await _priceListService.GetPriceListByIdAsync(id);
            if (priceListDTO == null)
            {
                return NotFound();
            }
            return View(priceListDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, UpdatePriceListDTO updatePriceListDTO)
        {
            if (id != updatePriceListDTO.Id)
            {
                return BadRequest();
            }
            if (ModelState.IsValid)
            {
                try
                {
                    await _priceListService.UpdatePriceListAsync(id, updatePriceListDTO);
                    TempData["SuccessMessage"] = $"Price List '{updatePriceListDTO.Name}' was updated successfully.";
                }
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(updatePriceListDTO);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> SetAsDefault(int id)
        {
            var result = await _priceListService.SetAsDefaultPriceListAsync(id);
            if (!result)
            {
                return NotFound();
            }
            TempData["SuccessMessage"] = "Default price list was updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ManageItems(int id)
        {
            var model = await _priceListService.GetPriceListItemsByIdAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            model.NewItem.ValidFrom = DateTime.Today;
            model.NewItem.ValidTo = DateTime.Today.AddYears(1);
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddItem(ManagePriceListItemsDTO model)
        {
            model.NewItem.PriceListId = model.PriceListId;
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Invalid data provided for the new price list item.";
                var refreshedModel = await _priceListService.GetPriceListItemsByIdAsync(model.PriceListId);
                if (refreshedModel == null)
                {
                    TempData["ErrorMessage"] = "The price list you are trying to modify could not be found.";
                    return RedirectToAction("Index");
                }
                refreshedModel.NewItem = model.NewItem;
                return View(nameof(ManageItems), refreshedModel);
            }
            try
            {
                await _priceListService.AddItemAsync(model.NewItem);
                TempData["SuccessMessage"] = "Price list item was added successfully.";
            }
            catch (KeyNotFoundException exception)
            {
                TempData["ErrorMessage"] = exception.Message;
            }
            catch (InvalidOperationException exception)
            {
                TempData["ErrorMessage"] = exception.Message;
            }
            return RedirectToAction(nameof(ManageItems), new { id = model.PriceListId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveItem(int id, int priceListId)
        {
            try
            {
                await _priceListService.RemoveItemAsync(id);
                TempData["SuccessMessage"] = "Price list item was removed successfully.";
            }
            catch (KeyNotFoundException exception)
            {
                TempData["ErrorMessage"] = exception.Message;
            }
            return RedirectToAction(nameof(ManageItems), new { id = priceListId });
        }

        [HttpGet]
        
        public async Task<IActionResult> SearchAvailableProducts(string? term)
        {
            var results = await _priceListService.SearchProductsAsync(term ?? "");
            var options = results.Select(p => new
            {
                value = p.Id,
                text = $"{p.SKU} - {p.Name}"
            });
            return Json(options);
        }
    }
}
