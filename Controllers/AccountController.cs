using DataVizNavigator1.Data;
using DataVizNavigator1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace DataVizNavigator1.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ApplicationDbContext context, ILogger<AccountController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            // If user is already logged in, redirect to home
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username and password match
                var user = _context.Users.FirstOrDefault(u =>
                    u.Username == model.Username &&
                    u.Password == model.Password);

                if (user != null)
                {
                    // Store user info in session
                    HttpContext.Session.SetString("Username", user.Username);
                    HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
                    HttpContext.Session.SetInt32("UserId", user.Id);

                    // Handle "Remember Me" functionality
                    if (model.RememberMe)
                    {
                        // Set a cookie that expires in 30 days
                        var cookieOptions = new CookieOptions
                        {
                            Expires = DateTime.Now.AddDays(30),
                            HttpOnly = true,
                            IsEssential = true
                        };

                        Response.Cookies.Append("RememberUsername", user.Username, cookieOptions);
                    }

                    // Redirect to home page after successful login
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Login failed
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public IActionResult Logout()
        {
            // Clear session
            HttpContext.Session.Clear();

            // Clear the remember me cookie if it exists
            if (Request.Cookies.ContainsKey("RememberUsername"))
            {
                Response.Cookies.Delete("RememberUsername");
            }

            return RedirectToAction("Login", "Account");
        }
    }
}