using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SalesStock.Application.Features.Orders.DTOs;
using SalesStock.Application.Interfaces;
using SalesStock.Domain.Enums;
using SalesStock.Web.ViewModels.Order;

namespace SalesStock.Web.Controllers
{
    [Authorize(Roles = "Admin,Manager,Operator")]
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICustomerService _customerService;
        private readonly IPriceListService _priceListService;

        public OrdersController(IOrderService orderService, ICustomerService customerService, IPriceListService priceListService)
        {
            _orderService = orderService;
            _customerService = customerService;
            _priceListService = priceListService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? customerSearchTerm,
            OrderStatus? status,
            DateTime? startDate,
            DateTime? endDate,
            int pageNumber = 1,
            int pageSize = 10)
        {
            ViewData["CurrentCustomerSearch"] = customerSearchTerm;
            ViewData["CurrentStatus"] = status;
            ViewData["CurrentStartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["CurrentEndDate"] = endDate?.ToString("yyyy-MM-dd");

            var pagedOrders = await _orderService.GetOrdersPagedAsync(
                customerSearchTerm, status, startDate, endDate, pageNumber, pageSize);

            return View(pagedOrders);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var customers = await _customerService.GetAllActiveCustomersForSelectListAsync();
            ViewBag.Customers = new SelectList(customers, "Id", "Name");

            var priceLists = await _priceListService.GetAllActivePriceListsForSelectListAsync();
            ViewBag.PriceLists = new SelectList(priceLists, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOrderDTO createOrderDTO)
        {
            if (!ModelState.IsValid)
            {
                var customers = await _customerService.GetAllActiveCustomersForSelectListAsync();
                var priceLists = await _priceListService.GetAllActivePriceListsForSelectListAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name", createOrderDTO.CustomerId);
                ViewBag.PriceLists = new SelectList(priceLists, "Id", "Name", createOrderDTO.PriceListId);
                return View(createOrderDTO);
            }

            try
            {
                int orderId = await _orderService.CreateDraftOrderAsync(createOrderDTO);

                return RedirectToAction("Edit", new { id = orderId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"An error occurred: {ex.Message}");

                var customers = await _customerService.GetAllActiveCustomersForSelectListAsync();
                var priceLists = await _priceListService.GetAllActivePriceListsForSelectListAsync();
                ViewBag.Customers = new SelectList(customers, "Id", "Name", createOrderDTO.CustomerId);
                ViewBag.PriceLists = new SelectList(priceLists, "Id", "Name", createOrderDTO.PriceListId);
                return View(createOrderDTO);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var orderDetails = await _orderService.GetOrderDetailsForEditAsync(id);

            if (orderDetails == null)
            {
                TempData["ErrorMessage"] = "Order not found.";
                return RedirectToAction("Index");
            }

            var viewModel = new OrderEditViewModel
            {
                Order = orderDetails,
                NewOrderItem = new AddOrderItemDTO { OrderId = id }
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddItem(AddOrderItemDTO orderItemDTO)
        {
            if (!ModelState.IsValid)
            {
                var errorMessages = ModelState.Values.SelectMany(v => v.Errors)
                                                 .Select(e => e.ErrorMessage)
                                                 .ToList();
                TempData["ErrorMessage"] = string.Join(" ", errorMessages);

                return RedirectToAction("Edit", new { id = orderItemDTO.OrderId });
            }

            try
            {
                await _orderService.AddItemToOrderAsync(orderItemDTO);
                TempData["SuccessMessage"] = "Item added successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error adding item: {ex.Message}";
            }
            return RedirectToAction("Edit", new { id = orderItemDTO.OrderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveItem(int orderId, int itemId)
        {
            try
            {
                await _orderService.RemoveItemAsync(orderId, itemId);
                TempData["SuccessMessage"] = "Item removed.";
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Remove failed.";
            }
            return RedirectToAction("Edit", "Orders", new { id = orderId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Approve(int id)
        {
            try
            {
                await _orderService.ApproveOrderAsync(id);
                TempData["SuccessMessage"] = "Order approved successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving order: {ex.Message}";
            }
            return RedirectToAction("Edit", new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Ship(int id)
        {
            try
            {
                await _orderService.ShipOrderAsync(id);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error shipping order: {ex.Message}";
            }
            return RedirectToAction("Edit", new { id });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                await _orderService.CancelOrderAsync(id);
                TempData["SuccessMessage"] = "Order cancelled successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error cancelling order: {ex.Message}";
            }
            return RedirectToAction("Index");
        }
    }
}