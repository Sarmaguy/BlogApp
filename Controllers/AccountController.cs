using Microsoft.AspNetCore.Mvc;
using BlogApp.Models;

namespace BlogApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly BlogDbContext _db;

        // Constructor with dependency injection
        public AccountController(BlogDbContext db)
        {
            _db = db;
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
            var user = _db.Users.FirstOrDefault(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                // Use TempData or a more modern session state mechanism
                TempData["UserId"] = user.Id;
                TempData["Username"] = user.Username;
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // GET: Logout
        public IActionResult Logout()
        {
            TempData.Clear();
            return RedirectToAction("Login");
        }
    }
}
