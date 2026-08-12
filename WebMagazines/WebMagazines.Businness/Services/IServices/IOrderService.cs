using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services.IServices
{
    // Create order endpoints
    public interface IOrderService
    {
        // Create Order endpoint
        Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader);

        // Get Order by Id endpoint
        // Set to nullable so it will return null when order id is invalid
        Task<OrderHeader?> GetOrderByIdAsync(int id, bool includeUser = false, bool includeDetails = false);

        // Get All Orders endpoint with optional filters for userId and status,
        // and options to include user and order details
        Task<IEnumerable<OrderHeader>> GetAllOrderAsync(string? userId=null, string? status=null, bool includeUser = false, bool includeDetails = false);

        // Update Order endopoint
        Task UpdateOrderAsync(OrderHeader orderHeader);

        // Update Order Status endopoint.
        // When set to shipped - carrier and tracking number must be updated. These don't need to be updated every time
        Task UpdateOrderStatusAsync(int id, string orderStatus, string? carrier = null, string? trackingNumber = null );
    }
}
