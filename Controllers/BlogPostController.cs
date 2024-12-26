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
                ? _db.BlogPosts
                    .Include(p => p.User)
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList()
                : _db.BlogPosts
                    .Include(p => p.User)
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.User)
                    .Where(p => p.Title.Contains(search))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

            ViewBag.SearchQuery = search; 
            ViewBag.CurrentUserId = HttpContext.Session.GetInt32("UserId");
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

        [HttpPost]
        public IActionResult AddComment(int blogPostId, string content)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 

            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("Content", "Comment content cannot be empty.");
                return RedirectToAction("Index");
            }

            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var comment = new Comment
            {
                BlogPostId = blogPostId,
                Content = content,
                UserId = userId.Value,
                CreatedAt = DateTime.Now
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        
        // [HttpPost, ActionName("Delete")]
        // public IActionResult DeleteComment(int id)
        // {
        //     var comment = _db.Comments.Find(id);
        //     if (comment == null) return NotFound();

        //     var userId = HttpContext.Session.GetInt32("UserId");
        //     if (userId == null || comment.UserId != userId)
        //     {
        //         return Unauthorized();
        //     }

        //     _db.Comments.Remove(comment);
        //     _db.SaveChanges();
        //     return RedirectToAction("Index", "BlogPost");
        // }

        [HttpPost]
        public IActionResult EditComment(int id, string content)
        {
            var comment = _db.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null) return NotFound();

            // Ensure the current user is the owner of the comment
            var userId = HttpContext.Session.GetInt32("UserId");
            if (comment.UserId != userId)
            {
                return Unauthorized();
            }

            // Update the comment
            comment.Content = content;
            comment.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            // Return updated comment data as JSON
            return Json(new
            {
                content = comment.Content,
                updatedAt = comment.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
            });
}



    }
}