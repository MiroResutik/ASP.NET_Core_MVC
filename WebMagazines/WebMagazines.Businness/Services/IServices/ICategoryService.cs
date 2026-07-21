using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services.IServices
{
    public interface ICategoryService
    {
        // Define a method to retrieve all categories asynchronously
        Task<Category?> GetCategoryByIdAsync(int id);
        // Define a method to retrieve all categories asynchronously
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        // Define a method to create a new category asynchronously
        Task<Category> CreateCategoryAsync(Category category);
        // Define a method to update an existing category asynchronously
        Task UpdateCategoryAsync(Category category);
        // Define a method to delete a category asynchronously
        Task DeleteCategoryAsync(int id);

        // Define a method to check if a category name is unique asynchronously
        Task<bool> IsCategoryNameUniqueAsync(string name, int? categoryId = null);
    }
}