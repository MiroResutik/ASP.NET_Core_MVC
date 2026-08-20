using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebMagazines.Businness.Services;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;

namespace WebMagazines.Areas.Admin.Controllers
{
    //[Area("Admin")] 
    [Authorize(Roles = SD.RoleAdmin+","+SD.RoleEmployee)]
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

        // Endpoint method for role management
        public async Task<IActionResult> RoleManagement(string userId)
        {
            // Retrieve user
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            RoleManagmentVM RoleVM = new()
            {
                ApplicationUser = user,
                RoleList = _roleManager.Roles.Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Text = u.Name,
                    Value = u.Name
                })
            };
            // Pupulate with default role for that user
            RoleVM.ApplicationUser.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            return View(RoleVM);
        }

        [HttpPost]
        // Endpoint method for role management
        public async Task<IActionResult> RoleManagement(RoleManagmentVM roleManagmentVM)
        {
            // Retrieve user
            var user = await _userService.GetUserByIdAsync(roleManagmentVM.ApplicationUser.Id);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            // Pupulate with default role for that user
            string oldRole = (await _userManager.GetRolesAsync(user)).FirstOrDefault();

            if (!(roleManagmentVM.ApplicationUser.Role == oldRole))
            {
                // Remove Role
                await _userManager.RemoveFromRoleAsync(user, oldRole);
                // Update to new role
                await _userManager.AddToRoleAsync(user, roleManagmentVM.ApplicationUser.Role);
            }

            TempData["Success"] = "Role has been updated";
            return RedirectToAction(nameof(Index));
        }
        // Endpoint method for password change management
        public async Task<IActionResult> ChangePassword(string userId)
        {
            // Retrieve user
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            AdminChangePasswordVM adminChangePasswordVM = new()
            {
                UserEmail = user.Email,
                UserId = user.Id
            };

            return View(adminChangePasswordVM);
        }
        [HttpPost]
        // Endpoint post method for password change management
        public async Task<IActionResult> ChangePassword(AdminChangePasswordVM adminChangePasswordVM)
        {
            if(!ModelState.IsValid)
            {
                return View(adminChangePasswordVM);
            }
            // Retrieve user - add userId to hidden property in the view
            var user = await _userService.GetUserByIdAsync(adminChangePasswordVM.UserId);

            if (user == null)
            {
                return NotFound(); // This can be change to redirect to different view
            }

            // Change password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user); // generates password token
            // Use helper function in user manager using token. It take 3 parameters
            var result = await _userManager.ResetPasswordAsync(user, token,adminChangePasswordVM.NewPassword);

            // 
            if (result.Succeeded)
            {
                TempData["Success"] = $"Password for {user.Email} has been changed successfully.";
                return RedirectToAction(nameof(Index));
            }
            
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            adminChangePasswordVM.UserEmail = user.Email;
            return View(adminChangePasswordVM);
            
        }
        #region API CALLS 

        // GET method to retrieve all users from the database and return them as JSON data
        [AllowAnonymous] // Allow anonymous access to the Index action
        public async Task<IActionResult> GetAll()
        {
            // Retrieve all users from the database using the ApplicationDbContext
            var users = await _userService.GetAllUsersAsync();

            // Get helper method in UserManager
            foreach(var user in users)
            {
                user.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            }
            return Json(new { data = users });
        }

        [HttpPost]
        public async Task<IActionResult> LockUnlock([FromBody] string userId)
        {
            // Retrieve user
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return Json(new {success = false, message = "User not found"});
            }
            // Get helper method in UserManager
            if (await _userManager.IsLockedOutAsync(user))
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
                return Json(new { success = true, message = "User locked successfully" });
            }else
            {
                await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddDays(1000));
                return Json(new { success = true, message = "User unlocked successfully" });

            }
            //return Json(new { data = user });
        }

        #endregion
    }
}
