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
            var users = _db.Users.ToList();
            return View(users);
        }

        // GET: /User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /User/Create
        [HttpPost]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                user.Password = _passwordHasher.HashPassword(user, user.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

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
                _db.SaveChanges();
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
            }
            return RedirectToAction("Index");
        }
    }
}
