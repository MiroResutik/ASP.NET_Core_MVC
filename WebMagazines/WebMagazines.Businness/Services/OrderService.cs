using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;
using WebMagazines.Utility;

namespace WebMagazines.Businness.Services
{
    public class OrderService : IOrderService
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _db;

        // Dependency injection of the ApplicationDbContext context through the constructor
        public OrderService(ApplicationDbContext db)
        {
            _db = db;
        }

        // Implement the CreateOrderAsync method to create a new order in the database
        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Add(orderHeader); // Add the new orderHeader to the OrderHeaders DbSet
            await _db.SaveChangesAsync(); // Save changes to the database asynchronously

            return orderHeader; // Return the created orderHeader
        }

        // Implement the GetAllOrderAsync method to retrieve
        // all orders with optional filters and inclusion of related entities
        public async Task<IEnumerable<OrderHeader>> GetAllOrderAsync(string? userId = null, string? status = null, bool includeUser = false, bool includeDetails = false)
        {
            // Start with the base query for OrderHeaders
            var query = _db.OrderHeaders.AsQueryable();

            // Include related entities based on the provided parameters
            if (includeUser)
            {
                query = query.Include(u => u.ApplicationUser); // Include the related ApplicationUser entity if requested
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails)
                             .ThenInclude(od => od.Product); // Include the related OrderDetails and Product entities if requested
            }
            // Apply filters based on the provided parameters
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                query = query.Where(o => o.OrderStatus.ToLower() == status.ToLower()); // Filter by status if provided
            }
            // Filter by userId if provided
            if (!string.IsNullOrEmpty(userId))
            {
                query = query.Where(o => o.ApplicationUserId == userId); // Filter by userId if provided
            }

            return await query.ToListAsync();
        }

        // Implement the GetOrderByIdAsync method to retrieve an order by its ID,
        // with optional inclusion of related entities
        public async Task<OrderHeader?> GetOrderByIdAsync(int id, bool includeUser = false, bool includeDetails = false)
        {
            // Start with the base query for OrderHeaders
            var query = _db.OrderHeaders.AsQueryable();

            // Include related entities based on the provided parameters
            if (includeUser)
            {
                query = query.Include(u => u.ApplicationUser); // Include the related ApplicationUser entity if requested
            }
            if (includeDetails)
            {
                query = query.Include(o => o.OrderDetails)
                             .ThenInclude(od => od.Product); // Include the related OrderDetails and Product entities if requested
            }
            // Retrieve the order by its ID, returning null if not found
            return await query.FirstOrDefaultAsync(o => o.Id == id);
        }

        // Implement the UpdateOrderAsync method to retrieve orderHeader and update the database
        public async Task UpdateOrderAsync(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Update(orderHeader);
            await _db.SaveChangesAsync();
        }

        // Implement the UpdateOrderStatusAsync method/interface 
        public async Task UpdateOrderStatusAsync(int id, string orderStatus, string? carrier = null, string? trackingNumber = null)
        {
            // Retrieve the order
            var order = await _db.OrderHeaders.FindAsync(id);

            // If the Order is null throw exception
            if (order == null)
            {
                throw new KeyNotFoundException($"Order {id} not found");
            }
            // Update order status that we revieved in parameters
            order.OrderStatus = orderStatus;

            // Check if order status is shipped
            if (orderStatus == SD.StatusShipped)
            {
                // Set shipping date
                order.ShippingDate = DateTime.UtcNow;
                // Check if carrier and tracking number empty and if so update them
                if(!string.IsNullOrEmpty(carrier))
                {
                    order.Carrier = carrier;
                }
                if(!string.IsNullOrEmpty(trackingNumber))
                {
                    order.TrackingNumber = trackingNumber;

                }
            }

            // Save changes
            await _db.SaveChangesAsync();
        }
    }
}
