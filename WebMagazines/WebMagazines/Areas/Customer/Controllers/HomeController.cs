using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Models;


namespace WebMagazines.Areas.Customer.Controllers
{
    //[Area("Customer")]
    public class HomeController : Controller
    {
        // Dependency injection of the IProductService and IShoppingCartService through the constructor
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;

        // Constructor to inject the IProductService and IShoppingCartService dependencies
        public HomeController(IProductService productService, IShoppingCartService shoppingCartService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
        }

        // Action method to display the list of products on the home page
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync(includeCategory: true);
            return View(products);
        }

        // Action method to display the details of a specific product based on the productId
        public async Task<IActionResult> Details(int productId)
        {
            // Retrieve the product details from the database using the productId
            var product = await _productService.GetProductByIdAsync(productId, includeCategory: true);
            if (product == null)
            {
                return NotFound();
            }

            // Create Shopping cart object to pass to the view
            ShoppingCart cart = new()
            {
                ProductId = productId,
                Product = product,
                Count = 1 // Default count value, this is default quantity for the product in the shopping cart
            };
            
            return View(cart);
        }

        // Action method to handle the form submission for adding items to the shopping cart
        [HttpPost] // Form submission for adding items to the shopping cart
        [Authorize] // User must be logged in to add items to the shopping cart
        public async Task<IActionResult> Details(ShoppingCart shoppingCart)
        {

            // Retrieve user Id from the claims of the logged in user 
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Check if the user is logged in and retrieve the user Id from the claims 
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(string.IsNullOrEmpty(userId))
            {

                return Unauthorized(); // User is not logged in or is unauthorized, return unauthorized response
            }
            // populate the application user Id in the shopping cart object with the logged in user's Id
            shoppingCart.ApplicationUserId = userId;

            // Add the shopping cart item to the database using the shopping cart service
            await _shoppingCartService.AddToCartAsync(shoppingCart);

            // Pause execution for 0.5 seconds on the server
            //await Task.Delay(500);

            // Set a success message in TempData to display on the next page
            TempData["success"] = $"{shoppingCart.Count} item(s) added to your cart.";

            

            // Redirect to the Details page of the product after adding it to the shopping cart
            return RedirectToAction("Index");
        }
    }
}
