using Microsoft.AspNetCore.Mvc;
using WebMagazines.Data;

namespace WebMagazines.Controllers
{
    public class CategoryController : Controller
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _context;
        // Dependency injection of the ApplicationDbContext context through the constructor
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            // Retrieve all categories from the database using the ApplicationDbContext
            var categories = _context.Categories.ToList();
            return View("Index", categories);
        }
    }
}
