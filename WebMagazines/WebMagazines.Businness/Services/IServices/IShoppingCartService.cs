using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services.IServices
{
    // Create shopping cart endpoints
    public interface IShoppingCartService
    {
        // Retrieve Cart Id
        // Set to nullable so it will return null when shopping cart id is invalid
        Task<ShoppingCart?> GetCartByIdAsync(int cartId);
        // Retrieve User cart items
        Task<IEnumerable<ShoppingCart>> GetUserCartItemsAsync(string userId);
        
        // Existing Count items in the shopping cart
        Task<int> GetCartCountAsync(string userId);

        // Add items to shopping cart and update item
        Task<ShoppingCart> AddToCartAsync(ShoppingCart cart);

        // Update shopping cart
        Task UpdateCartAsync(ShoppingCart cart);

        // clear shopping cart for specific userId
        Task ClearCartAsync(string userId);
    }
}
