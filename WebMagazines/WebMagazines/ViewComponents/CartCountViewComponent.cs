using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Utility;

namespace WebMagazines.ViewComponents
{
    public class CartCountViewComponent : ViewComponent
    {
        // Inject Shopping cart service in the constructor
        private readonly IShoppingCartService _shoppingCartService;

        public CartCountViewComponent(IShoppingCartService shoppingCartService)
        {
            _shoppingCartService = shoppingCartService;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Check if the user is Authenticated
            if (!User.Identity.IsAuthenticated)
            {
                // Only remove particular session not all sessions in the application
                HttpContext.Session.Remove(SD.SessionCart);
                return View(0); // If the user is not Authenticated the shopping count will be set to 0
            }

            // Retrieve user Id from the claims of the logged in user 
            var claimsIdentity = (ClaimsIdentity)User.Identity;

            // Check if the user is logged in and retrieve the user Id from the claims 
            var claim = claimsIdentity?.FindFirst(ClaimTypes.NameIdentifier);

            // If the claim is null
            if (claim == null)
            {
                // Only remove particular session not all sessions in the application
                HttpContext.Session.Remove(SD.SessionCart);
                return View(0); // If the user is not Authenticated the shopping count will be set to 0

            }

            // Retrieve the user Id value
            var sessionCount = HttpContext.Session.GetInt32(SD.SessionCart);

            if (sessionCount.HasValue)
            {

                return View(sessionCount.Value);
            }

            // If the session does not have value - retrieve it from database
            var cartCount = await _shoppingCartService.GetCartCountAsync(claim.Value); // Get the user Id from claim.value
            // Set the shopping cart session again
            HttpContext.Session.SetInt32(SD.SessionCart, cartCount);
            return View(cartCount);
        }
    }
}
