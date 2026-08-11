using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services
{
    // Endpoints for the CategoryService class that implements the ICategoryService interface
    public class CategoryService : ICategoryService
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _context;

        // Dependency injection of the ApplicationDbContext context through the constructor
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Method to get all categories asynchronously from the database
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Method to get a category by its ID asynchronously from the database
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Method to create a new category asynchronously in the database
        public async Task<Category> CreateCategoryAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        // Method to delete a category by its ID asynchronously from the database
        public async Task DeleteCategoryAsync(int id)
        {
            // Find the category by its ID
            var category = _context.Categories.Find(id);

            // If the category is not found, throw a Exception
            if (category == null)
            {
                throw new KeyNotFoundException($"Category with id {id} not found.");
            }
            // Remove the category from the database and save changes
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        // Method to update an existing category asynchronously in the database
        public async Task UpdateCategoryAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Method to check if a category name is unique asynchronously in the database
        public async Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null)
        {
            if (categoryId.HasValue)
            {
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower() && c.Id != categoryId.Value);
            }
            else
            {
                return !await _context.Categories.AnyAsync(c => c.Name.ToLower() == name.ToLower());

            }
        }
    }
}

