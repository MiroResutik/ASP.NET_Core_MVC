using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services
{
    // Endpoints for the ProductService class that implements the IProductService interface
    public class ProductService : IProductService
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _context;

        // Dependency injection of the ApplicationDbContext context through the constructor
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get all products, with an optional parameter to include related Category data
        public async Task<IEnumerable<Product>> GetAllProductsAsync(bool includeCategory=false)
        {
            // If includeCategory is true, use Include to load the related Category data for each Product
            if (includeCategory)
            {
                // Use Include to load the related Category data for each Product
                return await _context.Products.Include(p => p.Category).ToListAsync();
            }
            else
            {
                // If includeCategory is false, just return the list of Products without loading related Category data
                return await _context.Products.ToListAsync();
            }

        }

        // Method to get a single product by its ID, with an optional parameter to include related Category data
        public async Task<Product?> GetProductByIdAsync(int id, bool includeCategory = false)
        {
            if (includeCategory)
            {
                return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
            }
            else
            {
                return await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            }
        }

        // Method to create a new product and save it to the database
        public async Task<Product> CreateProductAsync(Product product)
        {
            _context.Products.Add(product); // Add the new product to the Products DbSet
            await _context.SaveChangesAsync();
            return product;
        }

        // Method to delete a product by its ID, throwing an exception if the product is not found
        public async Task DeleteProductAsync(int id)
        {
            // Find the product by its ID in the database
            var product = _context.Products.Find(id);
            if (product == null)
            {
                // If the product is not found, throw a Exception with indicating that the product with the specified ID was not found
                throw new KeyNotFoundException($"Product with id {id} not found.");
            }
            _context.Products.Remove(product); // Remove the product from the Products DbSet
            await _context.SaveChangesAsync();
        }

        // Method to update an existing product in the database
        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
    }
}

