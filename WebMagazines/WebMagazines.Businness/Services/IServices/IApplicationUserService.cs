using System;
using System.Collections.Generic;
using System.Text;
using WebMagazines.Models;

namespace WebMagazines.Businness.Services.IServices
{
    public interface IApplicationUserService
    {
        // Method to retrieve a user by their ID asynchronously
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
    }
}
