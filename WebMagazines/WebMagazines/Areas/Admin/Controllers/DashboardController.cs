using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebMagazines.DataAccess.Data;
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


    }
}
