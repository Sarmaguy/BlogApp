using Microsoft.AspNetCore.Mvc;
using BlogApp.Models;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;

        public AccountController(BlogDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
        }

        // GET: Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Username == username);
            if (user != null)
            {
                var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);
                if (result == PasswordVerificationResult.Success)
                {
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    HttpContext.Session.SetString("Username", user.Username);

                    TempData["SuccessMessage"] = "You have successfully logged in.";

                    return RedirectToAction("Index", "BlogPost");
                }
            }

            ViewBag.Error = "Invalid username or password.";
            TempData["ErrorMessage"] = "Invalid username or password.";
            return View();
        }

        // GET: Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            TempData["SuccessMessage"] = "You have successfully logged out.";
            return RedirectToAction("Login");
        }
    }
}
