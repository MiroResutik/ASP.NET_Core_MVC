using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mailjet.Client.Resources;
using Stripe;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;
using WebMagazines.Business.Services.IServices;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;


namespace WebMagazines.Areas.Customer.Controllers
{
    //[Area("Customer")]
    [Authorize] // Ensure that only authenticated users can access the CartController]
    public class CartController : Controller
    {
        // Dependency injection of the IProductService,IShoppingCartService, IEmailService
        // IApplicationUserService, and IOrderService through the constructor 
        private readonly IProductService _productService;
        private readonly IEmailService _emailService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IApplicationUserService _applicationUserService;
        private readonly IOrderService _orderService;

        // Cart Constructor to inject the IProductService,IShoppingCartService,
        // IApplicationUserService, and IOrderService dependencies
        public CartController(
            IProductService productService, 
            IShoppingCartService shoppingCartService, 
            IApplicationUserService applicationUserService, 
            IOrderService orderService, 
            IEmailService emailService)
        {
            _productService = productService;
            _emailService = emailService;
            _shoppingCartService = shoppingCartService;
            _applicationUserService = applicationUserService;
            _orderService = orderService;
        }

        // Action method to display the shopping cart for the authenticated user
        public async Task<IActionResult> Index()
        {
            // Retrieve the userId from the authenticated user's claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // If the userId is null or empty, redirect the user to the login page
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

            // Calculate the total order amount based on the cart items and their quantities
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                //cart.Product = await _productService.GetProductByIdAsync(cart.ProductId);
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            return View(shoppingCartVM); // Return the ShoppingCartVm instance to the view for rendering
        }

        // Action method to handle the POST request for the shopping cart form submission
        [HttpPost] // Handle the POST request for the shopping cart form submission
        [ActionName("Index")] // Specify the action name for the POST request to match the GET request
        public async Task<IActionResult> IndexPost(ShoppingCartVM shoppingCartVM)
        {
            // Retrieve the userId from the authenticated user's claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Retrieve the shopping cart items for the authenticated user using the userId
            var cartItems = await _shoppingCartService.GetUserCartItemsAsync(userId);

            // Populate the ShoppingCartVM with the retrieved cart items and user details
            shoppingCartVM.ShoppingCartList = cartItems;
            shoppingCartVM.OrderHeader.OrderDate = DateTime.Now;
            shoppingCartVM.OrderHeader.ApplicationUserId = userId;
     

            // Calculate the total order amount based on the cart itmes and their quantities
            foreach (var cart in shoppingCartVM.ShoppingCartList)
            {
                shoppingCartVM.OrderHeader.OrderTotal += (cart.Price * cart.Count);
            }

            // Set the order status to "Approved" for the new order
            shoppingCartVM.OrderHeader.OrderStatus = SD.StatusPending;

            // Create a list of OrderDetails based on the shopping cart items using projection with LINQ
            shoppingCartVM.OrderHeader.OrderDetails = shoppingCartVM.ShoppingCartList
                .Select(cart => new OrderDetails
            {
                ProductId = cart.ProductId,
                Count = cart.Count,
                Price = cart.Price
            }).ToList();

            // Create Order

            // Call the CreateOrderAsync method of the IOrderService to create a new order based on the ShoppingCartVM
            await _orderService.CreateOrderAsync(shoppingCartVM.OrderHeader);

            // Stripe session object - Gather the payment before Email service
            
            try
            {
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                var options = new Stripe.Checkout.SessionCreateOptions
                {
                    // Stripe will redirect if payment is successful
                    SuccessUrl = domain+ $"cart/OrderConfirmation?id={shoppingCartVM.OrderHeader.Id}",
                    CancelUrl =domain+ "cart/index", // Redirect if user cancels the payment
                    LineItems = new List<SessionLineItemOptions>(),

                    Mode = "payment",
                    // Metadata links the stripe session back to internal orderId - essential for debugging
                    Metadata = new Dictionary<string, string>
                    {
                        {"OrderId", shoppingCartVM.OrderHeader.Id.ToString() }
                    }
                };

                foreach (var item in shoppingCartVM.ShoppingCartList)
                {

                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100),
                            Currency = "gbp",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },


                        Quantity = item.Count,

                    };
                    options.LineItems.Add(sessionLineItem);
                }
                // Creating object of a session service that handles the checkout session on the stripe server
                var service = new SessionService();
                // Sending payment info to Stripe API by creating a checkout session which returns
                // object with payment ID and URL 
                Session session = service.Create(options);

                await _orderService.UpdateStripePaymentAsync(shoppingCartVM.OrderHeader.Id, session.Id, session.PaymentIntentId);
                // Redirect to Stripe to collect the payment
                Response.Headers.Append("Location", session.Url);
                return new StatusCodeResult(303);


            }
            catch (Exception ex)
            {

                TempData["Error"] = "Payment processing failed. Please try again.";
                return RedirectToAction(nameof(Index));
            }
            
        }

        // Action method to display the order confirmation page after a successful order placement
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            // Retrieve the order header from DB based on the id 
            var orderHeader = await _orderService.GetOrderByIdAsync(id, includeUser: true);

            if(orderHeader == null)
            {
                return NotFound();
            }

            // Retrieve the userId from the authenticated user's claims
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
                
            }

            // Check if applicationUserId is the same as userId
            if(orderHeader.ApplicationUserId != userId)
            {
                return RedirectToAction("AccessDenied", "Account", new { area = "Identity" });
            }

            try
            {
                // Stripe Payment validation

                var service = new SessionService();
                Session session = service.Get(orderHeader.SessionId);
                // Check the Stripe Payment Status
                if (session.PaymentStatus.ToLower() == "paid")
                {
                    await _orderService.UpdateStripePaymentAsync(id, session.Id, session.PaymentIntentId);
                    // Update the order status
                    await _orderService.UpdateOrderStatusAsync(id, SD.StatusApproved);
                    TempData["Success"] = "Payment completed successfully! Your order has been confirmed.";
                }
                else
                {
                    TempData["Error"] = "Payment status is pending. Please contact support if you completed the payment.";

                }

            }
            catch (Exception ex)
            {

                TempData["Error"] = "Unable to verify payment status. Please contact support with your order number.";
            }

            // Email service 
            var user = await _applicationUserService.GetUserByIdAsync(userId);

            await _emailService.SendOrderConfirmationEmailAsync(toEmail: user.Email, orderHeader.Id, (decimal)orderHeader.OrderTotal);
            // Clear the shopping cart for the user after creating the order
            // await _shoppingCartService.ClearUserCartAsync(userId);

            // Redirect the user to the OrderConfirmation action with the order ID as a route parameter
            //return RedirectToAction("OrderConfirmation", new { id = shoppingCartVM.OrderHeader.Id });

            return View(id);
        }

        // Action method to increase the quantity of an item in the shopping cart
        public async Task<IActionResult> Plus(int cartId)
        {
            // Retrieve the cart item using the cartId
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);

            // If the cart item exists, increase the count of the cart item and save the changes
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

                    
                    await _shoppingCartService.UpdateCartAsync(cart); // Update the cart item in the database
                    await UpdateCartSessionAsync();

                }
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to decrease the quantity of an item in the shopping cart
        public async Task<IActionResult> Minus(int cartId)
        {
            // Retrieve the cart item using the cartId
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);

            // If the cart item exists, decrease the count of the cart item and save the changes
            if (cart != null)
            {
                cart.Count--;
                await _shoppingCartService.UpdateCartAsync(cart);
                await UpdateCartSessionAsync();
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
                await UpdateCartSessionAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        // Action method to update an item in the shopping cart
        public async Task<IActionResult> UpdateCart(int cartId, int count)
        {
            // Retrieve the cart item using the cartId
            var cart = await _shoppingCartService.GetCartByIdAsync(cartId);

            if (cart == null)
            {
                return NotFound();
            }

            // Update the count of the cart item based on the provided count value
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

            await _shoppingCartService.UpdateCartAsync(cart); // Update the cart item in the database
            await UpdateCartSessionAsync();
            return Ok(new { success = true }); // Return a JSON response indicating success
        }

        private async Task UpdateCartSessionAsync()
        {
            // Retrieve user Id from the claims of the logged in user 
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Check if the user is logged in and retrieve the user Id from the claims 
            var userId = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {

                var count = await _shoppingCartService.GetCartCountAsync(userId);

                // Add value to cart session
                HttpContext.Session.SetInt32(SD.SessionCart, count);
            }


        }
    }
}
