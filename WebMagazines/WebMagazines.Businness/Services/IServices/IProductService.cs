using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services.IServices
{
    public interface IProductService
    {
        // Define a method to retrieve all products asynchronously
        Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false);
        // Define a method to retrieve all products asynchronously, with an optional parameter to include category information
        Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false);
        // Define a method to create a new product asynchronously
        Task<Product> CreateProductAsync(Product product);
        // Define a method to update an existing product asynchronously
        Task UpdateProductAsync(Product product);
        // Define a method to delete a product asynchronously
        Task DeleteProductAsync(int id);


    }
}