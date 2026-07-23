using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;


namespace WebMagazines.Areas.Customer.Controllers
{
    public class HomeController : Controller
    {
        //[Area("Customer")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

    }
}
