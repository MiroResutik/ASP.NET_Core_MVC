using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.DataAccess.Data;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        // Define a private readonly field for the ApplicationDbContext
        private readonly ApplicationDbContext _context;
        // Dependency injection of the ApplicationDbContext context through the constructor
        public ApplicationUserService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            // Use the _context to query the Users DbSet and find the user by their Id
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
