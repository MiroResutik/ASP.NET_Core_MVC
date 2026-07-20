using Microsoft.AspNetCore.Mvc;
using WebMagazines.Data;
using WebMagazines.Models;

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

        public IActionResult Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Create")] // Specify the action name for the POST method
        public IActionResult CreatePOST(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) && _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower()))
            {
                ModelState.AddModelError("", "Category name already exists!!!");
                //return View(category);
            }
            if (ModelState.IsValid)
            {

                // Add the new category to the database using the ApplicationDbContext 
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");

            }
            return View(category);
        }
        public IActionResult Update(int? id)
        {
            if(id== null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the category from the database using the ApplicationDbContext
            var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            if(category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Update")] // Specify the action name for the POST method
        public IActionResult UpdatePOST(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) && _context.Categories.Any(c => c.Name.ToLower() == category.Name.ToLower() && c.Id != category.Id))
            {
                ModelState.AddModelError("", "Category name already exists!!!");
                //return View(category);
            }
            if (ModelState.IsValid)
            {

                // Add the new category to the database using the ApplicationDbContext 
                _context.Categories.Update(category);
                _context.SaveChanges();
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");

            }
            return View(category);
        }
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the category from the database using the ApplicationDbContext
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Delete")] // Specify the action name for the POST method
        public IActionResult DeletePOST(int id)
        {
            var category = _context.Categories.Find(id);
            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            _context.SaveChanges();
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
        
    }
}
