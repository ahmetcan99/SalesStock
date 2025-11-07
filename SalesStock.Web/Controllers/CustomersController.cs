using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesStock.Infrastructure.Persistence;
using SalesStock.Domain.Entities;
using SalesStock.Web.Common;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string currentFilter, string searchTerm, string statusFilter, int? pageNumber)
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

            var query = _context.Customers.AsNoTracking();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(c => c.Code.Contains(searchTerm)
                                       || c.Name.Contains(searchTerm)
                                       || (c.Email != null && c.Email.Contains(searchTerm)));
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
                case "name_desc":
                    query = query.OrderByDescending(c => c.Name);
                    break;
                case "Code":
                    query = query.OrderBy(c => c.Code);
                    break;
                case "code_desc":
                    query = query.OrderByDescending(c => c.Code);
                    break;
                case "Email":
                    query = query.OrderBy(c => c.Email);
                    break;
                case "email_desc":
                    query = query.OrderByDescending(c => c.Email);
                    break;
                case "Status":
                    query = query.OrderBy(c => c.IsActive);
                    break;
                case "status_desc":
                    query = query.OrderByDescending(c => c.IsActive);
                    break;
                default:
                    query = query.OrderBy(c => c.Name);
                    break;
            }

            int pageSize = 10;
            int currentPage = pageNumber ?? 1;

            var pagedList = await PaginatedList<Customer>.CreateAsync(query, currentPage, pageSize);

            return View(pagedList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,Name,Email,Phone,IsActive")] Customer customer)
        {
            if (await _context.Customers.AnyAsync(c => c.Code == customer.Code))
            {
                ModelState.AddModelError("Code", "Customer code must be unique.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(customer);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Customer '{customer.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Code,Name,Email,Phone,IsActive,CreatedAt")] Customer customer)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }
            if (await _context.Customers.AnyAsync(c => c.Code == customer.Code && c.Id != customer.Id))
            {
                ModelState.AddModelError("Code", "Customer code must be unique.");
            }
            if (ModelState.IsValid)
            {
                try
                {
                    customer.UpdatedAt = DateTime.UtcNow;
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                    
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Customers.Any(e=> e.Id == id ))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = $"Customer '{customer.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                TempData["ErrorMessage"] = "Customer not found.";
                return RedirectToAction(nameof(Index));
            }

            customer.IsActive = !customer.IsActive;
            _context.Update(customer);
            await _context.SaveChangesAsync();

            var status = customer.IsActive ? "activated" : "deactivated";
            TempData["SuccessMessage"] = $"Customer '{customer.Name}' has been {status}.";

            return RedirectToAction(nameof(Index));
        }
    }
}
