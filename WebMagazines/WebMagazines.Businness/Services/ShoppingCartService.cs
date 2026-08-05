using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services
{
    // Endpoint to implement Shopping Cart Service
    public class ShoppingCartService : IShoppingCartService
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _context;
        // Dependency injection of the ApplicationDbContext context through the constructor
        public ShoppingCartService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Recieving shopping cart object
        public async  Task<ShoppingCart> AddToCartAsync(ShoppingCart cart)
        {
            // Retrieve any item from database provided procut ID and user ID are matching
            var existingItem = await _context.ShoppingCarts
                .Include(u => u.Product)
                .FirstOrDefaultAsync(u => u.ApplicationUserId == cart.ApplicationUserId && u.ProductId==cart.ProductId);
            // If no matching entry is found - there is no existing item
            // we need to create a record in the shopping cart table
            if (existingItem != null)
            {
                existingItem.Count += cart.Count;
                await _context.SaveChangesAsync(); // Save the changes to shopping cart
                return existingItem;
            }
            else
            {
                _context.ShoppingCarts.Add(cart);
                await _context.SaveChangesAsync();
                return cart;
            }
        }

        // Clear shopping cart
        public async Task ClearCartAsync(string userId)
        {
            var cartItems = await _context.ShoppingCarts
                .Include(u => u.Product)
                .Where(u => u.ApplicationUserId == userId)
                .ToListAsync();
            // Chceck whetere there are any items in this perticular cart
            if(cartItems.Any())
            {
                // Remove items form the cart
                _context.ShoppingCarts.RemoveRange(cartItems);
                _context.SaveChangesAsync();
            }
        }

        public async Task<ShoppingCart?> GetCartByIdAsync(int cartId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).FirstOrDefaultAsync(u => u.Id == cartId);
        }

        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(u => u.ApplicationUserId == userId).SumAsync(u => u.Count);
        }
        // Alternative to use .count rather than Sum of items
        /*
        public async Task<int> GetCartCountAsync(string userId)
        {
            return await _context.ShoppingCarts.Where(u => u.ApplicationUserId == userId).Count(u => u.Count);
        }
        */

        // Get all items in the shopping cart for a specific user
        public async Task<IEnumerable<ShoppingCart>> GetUserCartItemsAsync(string userId)
        {
            return await _context.ShoppingCarts.Include(u => u.Product).Where(u => u.ApplicationUserId == userId).ToListAsync();

        }

        // Update the shopping cart
        public async Task UpdateCartAsync(ShoppingCart cart)
        {
            if(cart.Count <= 0)
            {
                _context.ShoppingCarts.Remove(cart);
            }
            else
            {
                _context.ShoppingCarts.Update(cart);
            }
            await _context.SaveChangesAsync();
        }
    }
}
