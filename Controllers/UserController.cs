using Microsoft.AspNetCore.Mvc;
using BlogApp.Models;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace BlogApp.Controllers
{
    public class UserController : Controller
    {
        private readonly BlogDbContext _db;
        private readonly PasswordHasher<User> _passwordHasher;

        public UserController(BlogDbContext db)
        {
            _db = db;
            _passwordHasher = new PasswordHasher<User>();
            
        }

        // GET: /User
        public IActionResult Index()
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to view users.";
                return RedirectToAction("Login", "Account");
            }

            var users = _db.Users.ToList();
            return View(users);
        }

        // GET: /User/Create
        public IActionResult Create()
        {

            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a user.";
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        public IActionResult Create(User user)
        {  
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                return Unauthorized();
            }
            if (ModelState.IsValid)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _db.Users.Add(user);

                try
                {
                    _db.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate key"))
                    {
                        //check in db if there is the user with same username
                        var existingUser = _db.Users.FirstOrDefault(u => u.Username == user.Username);
                        if (existingUser != null)
                        {
                            TempData["ErrorMessage"] = "Username already exists! Please try again.";
                            user.Username = null;
                            return View(user);
                        }
                        else {
                            TempData["ErrorMessage"] = "Email already exists! Please try again.";
                            user.Email = null;
                            return View(user);
                        }
                    }

                    TempData["ErrorMessage"] = "User creation failed! Please try again.";
                    return View(user);
                }


                TempData["SuccessMessage"] = "User created successfully!";

                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "User creation failed! Please try again.";

            return View(user);
        }

        // GET: /User/Edit/id
        public IActionResult Edit(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /User/Edit/id
        [HttpPost]
        public IActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _db.Users.Update(user);

                try {
                    _db.SaveChanges();
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    if (ex.InnerException.Message.Contains("duplicate key"))
                    {
                        var existingUser = _db.Users.FirstOrDefault(u => u.Username == user.Username);
                        if (existingUser != null)
                        {
                            TempData["ErrorMessage"] = "Username already exists! Please try again.";
                            user.Username = null;
                            return View(user);
                        }
                        else {
                            TempData["ErrorMessage"] = "Email already exists! Please try again.";
                            user.Email = null;
                            return View(user);
                        }
                    }

                    TempData["ErrorMessage"] = "User update failed! Please try again.";
                    return View(user);
                }

                TempData["SuccessMessage"] = "User updated successfully!";

                return RedirectToAction("Index");
            }
            return View(user);
        }

        // GET: /User/Delete/id
        public IActionResult Delete(int id)
        {
            var user = _db.Users.Find(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        // POST: /User/Delete/id
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var user = _db.Users.Find(id);
            if (user != null)
            {
                _db.Users.Remove(user);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "User deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "User deletion failed! Please try again.";
            }
            return RedirectToAction("Index");
        }
    }
}
