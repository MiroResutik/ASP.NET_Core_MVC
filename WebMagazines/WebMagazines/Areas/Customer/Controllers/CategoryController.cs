using Microsoft.AspNetCore.Mvc;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;

namespace WebMagazines.Areas.Customer.Controllers
{
    //[Area("Customer")]
    public class CategoryController : Controller
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ICategoryService _categoryService;
        // Dependency injection of the ApplicationDbContext context through the constructor
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        public async Task<IActionResult> Index()
        {
            // Retrieve all categories from the database using the ApplicationDbContext
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        public async Task<IActionResult> Create()
        {

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Create")] // Specify the action name for the POST method
        public async Task<IActionResult> CreatePOST(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) && 
                !await _categoryService.IsCategoryNameUniqueAsync(category.Name))
            {
                ModelState.AddModelError("", "Category name already exists!!!");
                //return View(category);
            }
            if (ModelState.IsValid)
            {

                // Add the new category to the database using the ApplicationDbContext 
                await _categoryService.CreateCategoryAsync(category);
                TempData["success"] = "Category created successfully";
                return RedirectToAction("Index");

            }
            return View(category);
        }
        public async Task<IActionResult> Update(int? id)
        {
            if(id== null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the category from the database using the ApplicationDbContext
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Update")] // Specify the action name for the POST method
        public async Task<IActionResult> UpdatePOST(Category category)
        {
            if (!String.IsNullOrEmpty(category.Name) &&
                !await _categoryService.IsCategoryNameUniqueAsync(category.Name, category.Id))
            {
                ModelState.AddModelError("", "Category name already exists!!!");
                //return View(category);
            }
            if (ModelState.IsValid)
            {

                // Add the new category to the database using the ApplicationDbContext 
                await _categoryService.UpdateCategoryAsync(category);
                TempData["success"] = "Category updated successfully";
                return RedirectToAction("Index");

            }
            return View(category);
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the category from the database using the ApplicationDbContext
            var category = await _categoryService.GetCategoryByIdAsync(id.Value);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Delete")] // Specify the action name for the POST method
        public async Task<IActionResult> DeletePOST(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            TempData["success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
        
    }
}
