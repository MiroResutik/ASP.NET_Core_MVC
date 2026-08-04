using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;


namespace WebMagazines.Areas.Customer.Controllers
{
    //[Area("Customer")]
    [Authorize] // Ensure that only authenticated users can access the CartController]
    public class CartController : Controller
    {
        // Dependency injection of the IProductService and IShoppingCartService through the constructor
        private readonly IProductService _productService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;

        // Cart Constructor to inject the IProductService and IShoppingCartService dependencies
        public CartController(IProductService productService, IShoppingCartService shoppingCartService, IApplicationUserService applicationUserService)
        {
            _productService = productService;
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
        }

        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
                //return RedirectToAction("Login", "Account", new { area = "Identity" });
            }

            // Retrieve the shopping cart items for the authenticated user using the userId
            var cartItems = await _shoppingCartService.GetUserCartItemsAsync(userId);

            // Retrieve the user details using the userId
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            // Create a ShoppingCartVM instance and populate it with the cart items and user details
            ShoppingCartVM shoppingCartVM = new()
            {
                ShoppingCartList = cartItems,
                OrderHeader = new()

            };

            // Populate the OrderHeader with user details
            shoppingCartVM.OrderHeader.ApplicationUser = user;
            shoppingCartVM.OrderHeader.ApplicationUserId = user.Id;
            shoppingCartVM.OrderHeader.Name = user.Name;
            shoppingCartVM.OrderHeader.PhoneNumber = user.PhoneNumber;
            shoppingCartVM.OrderHeader.StreetAddress = user.StreetAddress;
            shoppingCartVM.OrderHeader.City = user.City;
            shoppingCartVM.OrderHeader.State = user.State;
            shoppingCartVM.OrderHeader.PostalCode = user.PostalCode;

            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                //cart.Product = await _productService.GetProductByIdAsync(cart.ProductId);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM);
        }

        // Action method to increase the quantity of an item in the shopping cart
        public async Task<IActionResult> Plus(int cartId)
        {

            // Retrieve the cart item using the cartId
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);

            if (cart != null)
            {
                // Ensure that the count does not exceed 1000
                if (cart.Count == 1000)
                {
                    // Optionally, you can display a message to the user indicating that the maximum quantity has been reached
                    TempData["ErrorMessage"] = "Maximum quantity reached for this item.";

                }
                else
                {
                    // Increase the count of the cart item and save the changes
                    cart.Count++;
                    await _shoppingCartService.UpdateCartAsync(cart);
                }
                

            }

            return RedirectToAction(nameof(Index));
        }
        // Action method to decrease the quantity of an item in the shopping cart
        public async Task<IActionResult> Minus(int cartId)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart != null)
            {
                cart.Count--;
                await _shoppingCartService.UpdateCartAsync(cart);
                //await UpdateCartSessionAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Action method to remove an item from the shopping cart
        public async Task<IActionResult> Remove(int cartId)
        {

            // Retrieve the cart item using the cartId
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);

            // If the cart item exists, remove it from the shopping cart
            if (cart != null)
            {
                cart.Count = 0;
                await _shoppingCartService.UpdateCartAsync(cart);
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to update an item in the shopping cart
        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);
            if (cart == null)
            {
                return NotFound();
            }


            if (count <= 1)
            {
                cart.Count = 0;
            }
            else
            {
                if (count >= 1000)
                {
                    cart.Count = 1000;
                }
                else
                {
                    cart.Count = count;
                }
            }
            await _shoppingCartService.UpdateCartAsync(cart);
            //await UpdateCartSessionAsync();
            return Ok(new { success = true });
        }
    }
}
