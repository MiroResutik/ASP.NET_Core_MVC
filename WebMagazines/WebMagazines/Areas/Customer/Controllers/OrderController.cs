using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;

namespace WebMagazines.Areas.Controllers
{
    //[Area("Customer")]
    // Add the Authorize attribute to restrict access to authenticated users only
    [Authorize(Roles = SD.RoleAdmin)] // Restrict access to users with the "Admin" role
    public class OrderController : Controller
    {
        // Define a private readonly field for the ApplicationDbContext product service
        private readonly IOrderService _orderService;

        [BindProperty]
        public OrderHeader OrderHeader { get; set; }

        // Constructor to initialize the order service, category service, and host environment
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // GET method to display the Index view with a list of products
        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // POST method 
        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, includeDetails: true, includeUser: true);
            return View(OrderHeader);
        }

        // POST method for Update Order Details
        [HttpPost]
        // Only Admin and Employee can update order details
        [Authorize(Roles = SD.RoleAdmin +","+ SD.RoleEmployee)] 
        
        public async Task<IActionResult> UpdateOrderDetails() // No need to pass OrderHeader as it is Bind Property. This will get automaticaly populated
        {
            var orderHeaderFromDb = await _orderService.GetOrderByIdAsync(OrderHeader.Id); // OrderHeader data pulled from database
            // List of Values that need to be updated explicitly
            orderHeaderFromDb.Name = OrderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderHeader.City;
            orderHeaderFromDb.State = OrderHeader.State;
            orderHeaderFromDb.PostalCode = OrderHeader.PostalCode;

            // Carrier and Tracking Number will not be updated every time 
            //This will only be required when changing the Order Status to shipped
            // if OrderHeader Carrier is empty we do not want to update the order only when orderHeaderFromDb is the same as statusShipped
            if (!string.IsNullOrEmpty(OrderHeader.Carrier) && orderHeaderFromDb.OrderStatus==SD.StatusShipped)
            {
                orderHeaderFromDb.Carrier = OrderHeader.Carrier;
            }
            // Same logic for tracking number
            if (!string.IsNullOrEmpty(OrderHeader.TrackingNumber) && orderHeaderFromDb.OrderStatus == SD.StatusShipped)
            {
                orderHeaderFromDb.TrackingNumber = OrderHeader.TrackingNumber;

            }

            // 
            await _orderService.UpdateOrderAsync(orderHeaderFromDb);

            // TODO:Success toast is not working currently
            TempData["Success"] = "Order Details Updated Successfully.";

            return RedirectToAction(nameof(Details), new { orderId = orderHeaderFromDb.Id });
        }

        // POST method for Update Order Status
        [HttpPost]
        // Only Admin and Employee can update status details
        [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]

        public async Task<IActionResult> UpdateOrderStatus(string status)
        {
            var orderHeader = await _orderService.GetOrderByIdAsync(OrderHeader.Id); // OrderHeader data pulled from database

            // Return error and redirect to Index page if OrderHeader is empty
            if (orderHeader == null) {
                TempData["Error"] = "Order not found.";
                return RedirectToAction(nameof(Index));
            }

            string successMessage;

            // Recieve all of the status

            switch(status)
            {
                case SD.StatusProcessing:
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                    successMessage = "Order processing started successfully.";
                    break;
                case SD.StatusCancelled:
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                    successMessage = "Order cancelled successfully.";
                    break;
                case SD.StatusRefunded:
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, status);
                    successMessage = "Order refunded successfully.";
                    break;
                case SD.StatusShipped:
                    if (string.IsNullOrEmpty(OrderHeader.Carrier) || string.IsNullOrEmpty(OrderHeader.TrackingNumber))
                    {
                        TempData["Error"] = "Please provide both carrier and tracking number.";
                        return RedirectToAction(nameof(Details), new {orderId = OrderHeader.Id});
                    }
                    await _orderService.UpdateOrderStatusAsync(OrderHeader.Id, SD.StatusShipped,OrderHeader.Carrier,OrderHeader.TrackingNumber);
                    successMessage = "Order shipped successfully.";
                    break;
                default:
                    TempData["Error"] = "Invalid status update.";
                    return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
            }
            // TODO:Success toast is not working currently
            TempData["Success"] = successMessage;

            return RedirectToAction(nameof(Details), new { orderId = OrderHeader.Id });
        }

        // POST method to handle the form submission for deleting a product
        #region API CALLS 

        // GET method to retrieve all products from the database and return them as JSON data
        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> GetAll(string status)
        {
            // Get the user ID from the claims if the user is authenticated
            string? userId = null;

            // Check if the user is not in the "Admin" or "Employee" roles
            if (!User.IsInRole(SD.RoleAdmin) && !User.IsInRole(SD.RoleEmployee))
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(); // Return 401 Unauthorized if the user is not authenticated
                }
            }
            // Retrieve all orders from the database using the ApplicationDbContext
            var orders = await _orderService.GetAllOrderAsync(userId,status);
            return Json(new { data = orders });
        }
        
        #endregion
    }
}
