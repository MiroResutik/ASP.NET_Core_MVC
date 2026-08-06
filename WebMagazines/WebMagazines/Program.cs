using Microsoft.EntityFrameworkCore;
using WebMagazines.Businness.Services.IServices;
using WebMagazines.Businness.Services;
using WebMagazines.DataAccess.Data;
using Microsoft.AspNetCore.Identity;
using WebMagazines.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Get the connection string from appsettings.json test
var test = builder.Configuration.GetConnectionString("SQLConnection");

// Set up default connection to Server SQL
builder.Services.AddDbContext<ApplicationDbContext>(
options => options.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection")));

// Set up default connection to Server SQL with sensitive data logging enabled.
// Uncomment the following code block if you want to enable sensitive data logging for debugging purposes.
// Be cautious when using this in production environments, as it may expose sensitive information in logs.
/*
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SQLConnection"));

    options.EnableSensitiveDataLogging();
});
*/

//// Set up Identity services with custom ApplicationUser and IdentityRole, and configure password requirements
//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();

// Add Identity services to the application and configure it
// to use the ApplicationDbContext for storing user and role information
// Configure password requirements for user accounts
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //options.SignIn.RequireConfirmedAccount = true;      // Require users to confirm their email before they can log in
    options.Password.RequireDigit = true;              // Must have at least one number (0-9)
    options.Password.RequireLowercase = true;          // Must have at least one lowercase letter (a-z)
    options.Password.RequireUppercase = true;          // Must have at least one uppercase letter (A-Z)
    options.Password.RequireNonAlphanumeric = true;    // Must have at least one special character (!@#$%^&*)
    options.Password.RequiredLength = 6;               // Minimum 6 characters
    options.Password.RequiredUniqueChars = 1;          // Minimum unique characters
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// Register the CategoryService with the dependency injection container
builder.Services.AddScoped<ICategoryService, CategoryService>();

// Register the ProductService with the dependency injection container
builder.Services.AddScoped<IProductService, ProductService>();

// Register the OrderService with the dependency injection container
builder.Services.AddScoped<IOrderService, OrderService>();

// Register the ShoppingCartService with the dependency injection container
builder.Services.AddScoped<IShoppingCartService, ShoppingCartService>();

// Register the ApplicationUserService with the dependency injection container
builder.Services.AddScoped<IApplicationUserService, ApplicationUserService>();


// Configure the application cookie settings for authentication
// This sets the paths for login, logout, and access denied pages,
// as well as the cookie expiration time
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = $"/Account/Login"; // Set the login path for unauthenticated users
    options.LogoutPath = $"/Account/Logout"; // Set the logout path for authenticated users
    options.AccessDeniedPath = $"/Account/AccessDenied"; // Set the access denied path for unauthorized users
    options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Set the cookie expiration time to 30 minutes
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Enable authentication and authorization middleware. Authentication must be called
// before authorization to ensure that the user is authenticated before checking their permissions.
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

// Configure the default route for the application, specifying the default controller and action
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { areas = "Customer" })
    .WithStaticAssets();

// Configure the route for areas, allowing for area-specific controllers and actions
app.MapControllerRoute(
    name: "areas",
    pattern: "{areas:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
