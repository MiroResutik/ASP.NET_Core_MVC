using Microsoft.EntityFrameworkCore;
using Stripe.Checkout;
using Stripe.Climate;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Business.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebMagazines.Business.Services
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

        public Task<bool> CancelOrderWithRefundAsync(int orderId)
        {
            throw new NotImplementedException();
        }

        // Implement the CreateOrderAsync method to create a new order in the database
        public async Task<OrderHeader> CreateOrderAsync(OrderHeader orderHeader)
        {
            _db.OrderHeaders.Add(orderHeader); // Add the new orderHeader to the OrderHeaders DbSet
            await _db.SaveChangesAsync(); // Save changes to the database asynchronously

            return orderHeader; // Return the created orderHeader
        }

        public async Task<string> CreateStripeCheckoutSessionAsync(OrderHeader orderHeader, IEnumerable<ShoppingCart> cartItems, string domain)
        {
            if (orderHeader == null)
            {
                throw new ArgumentNullException(nameof(orderHeader));
            }
            if (cartItems == null || !cartItems.Any())
            {
                throw new ArgumentException("Cart items cannot be empty", nameof(cartItems));
            }

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                // Stripe will redirect if payment is successful
                SuccessUrl = domain + $"cart/OrderConfirmation?id={orderHeader.Id}",
                CancelUrl = domain + "cart/index", // Redirect if user cancels the payment
                LineItems = new List<SessionLineItemOptions>(),

                Mode = "payment",
                // Metadata links the stripe session back to internal orderId - essential for debugging
                Metadata = new Dictionary<string, string>
                    {
                        {"OrderId", orderHeader.Id.ToString() }
                    }
            };

            foreach (var item in cartItems)
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

            await UpdateStripePaymentAsync(orderHeader.Id, session.Id, session.PaymentIntentId);

            return session.Url;
        }

        // Implement the GetAllOrderAsync method to retrieve
        // all orders with optional filters and inclusion of related entities
        public async Task<IEnumerable<OrderHeader>> GetAllOrderAsync(string? userId = null, string? status = null, bool includeUser = true, bool includeDetails = false)
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
                if (!string.IsNullOrEmpty(carrier))
                {
                    order.Carrier = carrier;
                }
                if (!string.IsNullOrEmpty(trackingNumber))
                {
                    order.TrackingNumber = trackingNumber;

                }
            }

            // Save changes
            await _db.SaveChangesAsync();
        }

        // Implement the UpdateStripePayment method/interface
        public async Task UpdateStripePaymentAsync(int orderId, string sessionId, string paymentIntentId)
        {
            // Retrieve the order
            var order = await _db.OrderHeaders.FindAsync(orderId);

            // If the Order is null throw exception
            if (order == null)
            {
                throw new KeyNotFoundException($"Order {orderId} not found");
            }
            // if the sessionId is not null or empty then update the order
            if (!string.IsNullOrEmpty(sessionId))
            {
                order.SessionId = sessionId;
            }
            // if the paymentIntentId is not null or empty then update the order
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                order.PaymentIntentId = paymentIntentId;
            }
            // Update the payment once the payment is successful in Stripe 
            await _db.SaveChangesAsync();
        }

        public async Task<bool> VerifyStripePaymentAsync(OrderHeader orderHeader)
        {
            // Stripe Payment validation

            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);
            // Check the Stripe Payment Status
            if (session.PaymentStatus.ToLower() == "paid")
            {
                await UpdateStripePaymentAsync(orderHeader.Id, session.Id, session.PaymentIntentId);
                // Update the order status
                await UpdateOrderStatusAsync(orderHeader.Id, SD.StatusApproved);
                return true;
            }
            else
            {
                return false;

            }
        }
    }
}
