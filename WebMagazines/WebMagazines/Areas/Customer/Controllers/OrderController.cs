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

        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> Details(int orderId)
        {
            OrderHeader = await _orderService.GetOrderByIdAsync(orderId, includeDetails: true, includeUser: true);
            return View(OrderHeader);
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
