using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SalesStock.Application.Features.Customers.DTOs;
using SalesStock.Application.Interfaces;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin, Manager, Operator")]
    public class CustomersController : Controller
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }
        public async Task<IActionResult> Index(string? sortOrder, string? currentFilter, string? searchTerm, string? statusFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParam"] = string.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CodeSortParam"] = sortOrder == "Code" ? "code_desc" : "Code";
            ViewData["EmailSortParam"] = sortOrder == "Email" ? "email_desc" : "Email";
            ViewData["StatusSortParam"] = sortOrder == "Status" ? "status_desc" : "Status";

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
            var pagedList = await _customerService.GetCustomersPagedAsync(sortOrder, searchTerm, statusFilter, pageNumber ?? 1, pageSize);

            return View(pagedList);
        }

        public IActionResult Create()
        {
            return View(new CreateCustomerDTO());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCustomerDTO customerDTO)
        {
            if (!ModelState.IsValid)
            {
                return View(customerDTO);
            }

            try
            {
                await _customerService.AddCustomerAsync(customerDTO);
                TempData["SuccessMessage"] = $"Customer '{customerDTO.Name}' created.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError("Code", exception.Message);
                return View(customerDTO);
            }
        }
        public async Task<IActionResult> Edit(int id)
        {
            var customerDTO = await _customerService.GetCustomerForUpdateAsync(id);
            if (customerDTO == null)
            {
                return NotFound();
            }
            return View(customerDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCustomerDTO customerDTO)
        {
            if (id != customerDTO.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(customerDTO);
            }

            try
            {
                await _customerService.UpdateCustomerAsync(customerDTO);
                TempData["SuccessMessage"] = $"Customer '{customerDTO.Name}' updated.";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException exception)
            {
                ModelState.AddModelError("Code", exception.Message);
                return View(customerDTO);
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
                await _customerService.ToggleCustomerStatusAsync(id);
                TempData["SuccessMessage"] = "Customer status updated.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Customer could not be found.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}