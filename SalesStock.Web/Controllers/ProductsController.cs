using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesStock.Application.Features.Products.DTOs;
using SalesStock.Application.Interfaces;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
         {
              _productService = productService;
        }

        public async Task<IActionResult> Index(string? sortOrder,string? currentFilter,string? searchTerm,string? statusFilter,int? pageNumber)
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

            int pageSize = 10;
            int currentPage = pageNumber ?? 1;
            var pagedList = await _productService.GetProductsPagedAsync(sortOrder ?? "name", searchTerm!, statusFilter!, currentPage, pageNumber ?? 1,pageSize);

            return View(pagedList);
        }


        public IActionResult Create()
        {
            var model = new CreateProductDTO
            {
                VatRate = 0.2m
            };
            return View(model);
                
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDTO ProductDTO)
        {
            if(!ModelState.IsValid)
            {
                return View(ProductDTO);
            }
            try
            {
                await _productService.AddProductAsync(ProductDTO);
                TempData["SuccessMessage"] = $"Product '{ProductDTO.Name}' was created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError("SKU", exception.Message);
                return View(ProductDTO);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var productDTO = await _productService.GetProductForUpdateAsync(id);
            if (productDTO == null) return NotFound();
            return View(productDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateProductDTO productDTO)
        {
            if (id != productDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(productDTO);
            }

            try
            {
                await _productService.UpdateProductAsync(productDTO);
                TempData["SuccessMessage"] = $"Product '{productDTO.Name}' updated succesfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError("Sku", exception.Message);
                return View(productDTO);
            }
            catch (KeyNotFoundException) 
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                await _productService.ToggleProductStatusAsync(id);
                TempData["SuccessMessage"] = "Product status updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
