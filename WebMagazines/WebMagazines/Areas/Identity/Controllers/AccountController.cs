using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebMagazines.Models;
using WebMagazines.Models.ViewModels;
using WebMagazines.Utility;

namespace WebMagazines.Views.Identity.Controllers
{
    public class AccountController : Controller
    {
        // Define private readonly fields for UserManager, SignInManager, and RoleManager
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Constructor to initialize UserManager, SignInManager, and RoleManager using dependency injection
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            // Initialize the private fields with the injected dependencies
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: /Account/Login
        // This action returns the login view
        public IActionResult Login(string? returnUrl = null)
        {
            // Store the return URL in ViewData to redirect the user after successful login
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: /Account/Login
        // This action handles the login of a user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginVM loginVM, string? returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Attempt to sign in the user using SignInManager
                var result = _signInManager.PasswordSignInAsync(loginVM.Email, 
                    loginVM.Password, loginVM.RememberMe, lockoutOnFailure: false).Result;

                // If the login is successful, redirect to the return URL or the home page
                if (result.Succeeded)
                {
                    // If a return URL is provided and it's a local URL, redirect to that URL
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new { area = "Customer" });

                }
                // If the login fails, add an error to the model state
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                
            }
            return View();
        }

        // GET: /Account/Register
        // This action returns the registration view
        public IActionResult Register(string? returnUrl = null)

        {
            // Create a new instance of RegisterVM and
            // populate the RoleList with available roles
            var model = new RegisterVM
            {
                // Populate the RoleList with available roles for registration
                RoleList =
                [
                    new SelectListItem { Text = SD.RoleCustomer, Value = SD.RoleCustomer },
                    new SelectListItem { Text = SD.RoleAdmin, Value = SD.RoleAdmin },
                    new SelectListItem { Text = SD.RoleEmployee, Value = SD.RoleEmployee }
                ]
            };
            // Store the return URL in ViewData to redirect the user after successful login
            ViewData["ReturnUrl"] = returnUrl;

            // Return the registration view with the model
            return View(model);
        }

        // POST: /Account/Register
        // This action handles the registration of a new user
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM registerVM, string? returnUrl = null)
        {
            // Check if the roles exist, and create them if they don't 
            if (!await _roleManager.RoleExistsAsync(SD.RoleCustomer))
            {
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleAdmin));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleCustomer));
                await _roleManager.CreateAsync(new IdentityRole(SD.RoleEmployee));
            }
            // Check if the model state is valid
            if (ModelState.IsValid)
            {
                // Create a new ApplicationUser instance with the provided registration details
                var user = new ApplicationUser { 
                    UserName = registerVM.Email, 
                    Email = registerVM.Email,
                    Name = registerVM.Name,
                    PhoneNumber = registerVM.PhoneNumber,
                    StreetAddress = registerVM.StreetAddress,
                    City = registerVM.City,
                    State = registerVM.State,
                    PostalCode = registerVM.PostCode
                };
                // Create the user using UserManager
                var result = await _userManager.CreateAsync(user, registerVM.Password);

                // If the user creation is successful, sign in the user and redirect to the home page
                if (result.Succeeded)
                {
                    // Assign the selected role to the user
                    if (!string.IsNullOrEmpty(registerVM.Role))
                    {
                        // Add the user to the selected role
                        await _userManager.AddToRoleAsync(user, registerVM.Role);
                    }
                    else
                    {
                        // If no role is selected, assign the default role (Customer)
                        await _userManager.AddToRoleAsync(user, SD.RoleCustomer);
                    }
                    // Sign in the user
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    // If a return URL is provided and it's a local URL, redirect to that URL
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new {area="Customer"});
                }
                // If there are errors, add them to the model state
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(registerVM);
        }

        // GET: /Account/AccessDenied
        // This action returns the access denied view
        public IActionResult AccessDenied()
        {
            return View();
        }

        // POST: /Account/Logout
        // This action handles the logout of the user
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
