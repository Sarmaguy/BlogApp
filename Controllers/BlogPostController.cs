using Microsoft.AspNetCore.Mvc;
using BlogApp.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BlogApp.Controllers
{
    public class BlogPostController : Controller
    {
        private readonly BlogDbContext _db;

        public BlogPostController(BlogDbContext db)
        {
            _db = db;
        }

        // GET: /BlogPost
        public IActionResult Index(string search = null)
        {
            var posts = string.IsNullOrEmpty(search) 
                ? _db.BlogPosts.ToList() 
                : _db.BlogPosts.Where(p => p.Title.Contains(search)).ToList();

            return View(posts);
        }

        // GET: /BlogPost/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /BlogPost/Create
        [HttpPost]
        public IActionResult Create(BlogPost post)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            post.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                _db.BlogPosts.Add(post);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                }


            return View(post);
        }


        // GET: /BlogPost/Edit/id
        public IActionResult Edit(int id)
        {
            var post = _db.BlogPosts.Find(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: /BlogPost/Edit/id
        [HttpPost]
        public IActionResult Edit(BlogPost post)
        {
            if (!ModelState.IsValid)
            {
                return View(post);
            }

            post.UserId = HttpContext.Session.GetInt32("UserId").Value;

            var existingPost = _db.BlogPosts.AsNoTracking().FirstOrDefault(p => p.Id == post.Id);
            if (existingPost == null)
            {
                return NotFound();
            }

            if (existingPost.UserId != post.UserId)
            {
                return Unauthorized();
            }

            post.UpdatedAt = DateTime.Now;
            _db.BlogPosts.Update(post);
            _db.SaveChanges();
            return RedirectToAction("Index");
        }

        // GET: /BlogPost/Delete/id
        public IActionResult Delete(int id)
        {
            var post = _db.BlogPosts.Find(id);
            if (post == null) return NotFound();

            return View(post);
        }

        // POST: /BlogPost/Delete/id
        [HttpPost, ActionName("Delete")]
        public IActionResult DeleteConfirmed(int id)
        {
            var post = _db.BlogPosts.Find(id);
            if (post != null)
            {
                _db.BlogPosts.Remove(post);
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}
