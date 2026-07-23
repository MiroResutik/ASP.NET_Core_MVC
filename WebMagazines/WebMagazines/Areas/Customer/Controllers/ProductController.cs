using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;

namespace WebMagazines.Areas.Controllers
{
    //[Area("Customer")]
    public class ProductController : Controller
    {
        // Define a private readonly field for the ApplicationDbContext product service
        private readonly IProductService _productService;
        // Define a private readonly field for the ApplicationDbContext category service
        private readonly ICategoryService _categoryService;
        // Define a private readonly field for the IWebHostEnvironment
        private readonly IWebHostEnvironment _hostEnvironment;

        // Constructor to initialize the product service, category service, and host environment
        public ProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment hostEnvironment)
        {
            _productService = productService;
            _categoryService = categoryService;
            _hostEnvironment = hostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            // Retrieve all products from the database using the ApplicationDbContext
            //var products = await _productService.GetAllProductsAsync();
            return View();
        }


        public async Task<IActionResult> Upsert(int? id)
        {
            // Retrieve all categories from the database using the ApplicationDbContext
            var categories = await _categoryService.GetAllCategoriesAsync();

            ProductVM productVM = new()
            {
                CategoryList = categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                }),
                Product = new Product()
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                // Retrieve the product from the database using the ApplicationDbContext
                productVM.Product = await _productService.GetProductByIdAsync(id.Value);
                return View(productVM);
            }
                
            
        }
        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Upsert")] // Specify the action name for the POST method
        public async Task<IActionResult> UpsertPOST(ProductVM productVM, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                // Handle file upload if a file is provided
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images\products");
                    var extension = Path.GetExtension(file.FileName);
                    if (productVM.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, productVM.Product  .ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        await file.CopyToAsync(fileStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                }

                if (productVM.Product.Id == null || productVM.Product.Id == 0)
                {
                    //create
                    await _productService.CreateProductAsync(productVM.Product);
                }
                else
                {
                    await _productService.UpdateProductAsync(productVM.Product);

                }
                TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");

            }
            else
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                 productVM = new()
                {
                    CategoryList = categories.Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }),
                    Product = new Product()
                };

                return View(productVM);
            }
           
        }
        // Update Not needed for product as Upsert is used for both create and update operations. The Update method is commented out below.
        /*
        public async Task<IActionResult> Update(int? id)
        {
            if(id== null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the product from the database using the ApplicationDbContext
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        [ActionName("Update")] // Specify the action name for the POST method
        public async Task<IActionResult> UpdatePOST(Product product)
        {

            if (ModelState.IsValid)
            {

                // Add the new product to the database using the ApplicationDbContext 
                await _productService.UpdateProductAsync(product);
                TempData["success"] = "Product updated successfully";
                return RedirectToAction("Index");

            }
            return View(product);
        }
        */

        //This is not needed because delete function is now handeled in product.js file
        //using ajax call to the api endpoint. The delete method is commented out below.
        // Delete post method to delete a product from the database
        /*
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            // Retrieve the product from the database using the ApplicationDbContext
            var product = await _productService.GetProductByIdAsync(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }
        */


        #region API CALLS

        public async Task<IActionResult> GetAllProducts()
        {
            // Retrieve all products from the database using the ApplicationDbContext
            var products = await _productService.GetAllProductsAsync(true);
            return Json(new { data = products });
        }

        //[HttpPost] // Use the HttpPost attribute to indicate that this action handles POST requests
        //[ValidateAntiForgeryToken] // Add this attribute to protect against Cross-Site Request Forgery (CSRF) attacks
        //[ActionName("Delete")] // Specify the action name for the POST method
        [HttpDelete] // Use the HttpDelete attribute to indicate that this action handles DELETE requests

        //public async Task<IActionResult> DeletePOST(int id)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return Json(new { success = false, message = "Invalid product ID" });
            }

            var productToBeDeleted = await _productService.GetProductByIdAsync(id.Value);
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "ErrorProduct not found" });
            }

            // Delete product image if the product has an associated image
            if (!string.IsNullOrEmpty(productToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\', '/'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }
            await _productService.DeleteProductAsync(id.Value);
            return Json(new { success = true, message = "Product deleted successfully" });
            //await _productService.DeleteProductAsync(id);
            //TempData["success"] = "Product deleted successfully";
            //return RedirectToAction("Index");
        }
        #endregion
    }
}
