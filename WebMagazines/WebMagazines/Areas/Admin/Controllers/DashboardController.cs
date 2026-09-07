using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;

namespace WebMagazines.Areas.Admin.Controllers
{
    //[Area("Admin")]
    [Authorize(Roles = SD.RoleAdmin + "," + SD.RoleEmployee)]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _db.OrderHeaders.ToListAsync();
            var productCount = await _db.Products.CountAsync();
            var userCount = await _db.Users.CountAsync();

            DashboardVM dashboardVM = new ()
            {
                TotalOrders = orders.Count,
                TotalProducts = productCount,
                TotalUsers = userCount,
                TotalRevenue = orders.Where(o => o.OrderStatus == SD.StatusApproved || o.OrderStatus == SD.StatusShipped)
                .Sum(o => o.OrderTotal)
            };

            return View(dashboardVM);
        }

        // Endpoint for all dashboard chart data - entity framework
        [HttpGet]
        public async Task<IActionResult> GetChartData()
        {
            // Retrieve the necessary data for the chart
            var orders = await _db.OrderHeaders.ToListAsync();
            var productCount = await _db.Products.Include(p => p.Category).ToListAsync();
            var categories = await _db.Categories.ToListAsync();
            var products = await _db.Products.Include(u => u.Category).ToListAsync();

            // Revenue by month - last 6 months
            var now = DateTime.UtcNow;
            var sixMonthsAgo = now.AddMonths(-5);
            var monthlyRevenue = Enumerable.Range(0, 6).Select(i =>
            {
                // Calculate the month for the current iteration
                var month = sixMonthsAgo.AddMonths(i);
                var revenue = orders
                .Where(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month
                && (o.OrderStatus == SD.StatusApproved || o.OrderStatus == SD.StatusShipped))
                .Sum(o => o.OrderTotal);

                return new { Label = month.ToString("MMM yyyy"), Revenue = revenue };
            }).ToList();

            // Orders by month (last 6 months)
            var monthlyOrders = Enumerable.Range(0, 6).Select(i =>
            {
                var month = sixMonthsAgo.AddMonths(i);
                var count = orders.Count(o => o.OrderDate.Year == month.Year && o.OrderDate.Month == month.Month);
                return new { Label = month.ToString("MMM yyyy"), Count = count };
            }).ToList();

            // Order status breakdown
            var statusBreakdown = orders
                .GroupBy(o => o.OrderStatus ?? "Unknown")
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToList();

            // Products per category
            var productsPerCategory = categories.Select(c => new
            {
                Category = c.Name,
                Count = products.Count(p => p.CategoryId == c.Id) // Count products in specific chosen category
            }).ToList();

            return Json(new
            {
                monthlyRevenue,
                monthlyOrders,
                statusBreakdown,
                productsPerCategory

            });
        }
    }
}
