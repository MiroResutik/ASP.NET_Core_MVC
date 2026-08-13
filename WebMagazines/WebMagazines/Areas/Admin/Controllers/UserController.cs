using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMagazines.Businness.Services;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Models;

namespace WebMagazines.Areas.Admin.Controllers
{
    //[Area("Admin")] 
    public class UserController : Controller
    {
        
        // Dependency injection for UserManager, RoleManager and IApplicationUserService
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IApplicationUserService _userService;

        // User control constructor to inject UserManager, RoleManager
        // and IApplicationUserService dependencies
        public UserController(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IApplicationUserService userService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }


        
        #region API CALLS 

        // GET method to retrieve all users from the database and return them as JSON data
        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> GetAll()
        {
            // Retrieve all products from the database using the ApplicationDbContext
            var users = await _userService.GetAllUsersAsync();

            // Get helper method in UserManager
            foreach(var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }
            return Json(new { data = users });
        }

        #endregion
    }
}
