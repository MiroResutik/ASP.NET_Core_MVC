using Microsoft.AspNetCore.Mvc;

namespace WebMagazines.Areas.Admin.Controllers
{
    //[Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
