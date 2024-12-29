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
            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to create a post.";
                return RedirectToAction("Login", "Account");
            }

            post.UserId = userId.Value;

            if (ModelState.IsValid)
            {
                post.CreatedAt = DateTime.Now;
                _db.BlogPosts.Add(post);
                _db.SaveChanges();

                TempData["SuccessMessage"] = "Post created successfully!";
                return RedirectToAction("Index");
            }

            if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);

                }

            TempData["ErrorMessage"] = "Post creation failed! Please try again.";


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
            // Validate the model
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Post update failed! Please try again.";
                return View(post);
            }

            post.UserId = HttpContext.Session.GetInt32("UserId").Value;

            var existingPost = _db.BlogPosts.AsNoTracking().FirstOrDefault(p => p.Id == post.Id);
            // Check if post exists
            if (existingPost == null)
            {
                return NotFound();
            }
            // Check if user is authorized to edit the post
            if (existingPost.UserId != post.UserId)
            {
                return Unauthorized();
            }

            post.CreatedAt = existingPost.CreatedAt;

            post.UpdatedAt = DateTime.Now;

            _db.BlogPosts.Update(post);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Post updated successfully!";
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
                TempData["SuccessMessage"] = "Post deleted successfully!";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddComment(int blogPostId, string content)
        {

            // Check if user is logged in
            var userId = HttpContext.Session.GetInt32("UserId"); 
            if (userId == null)
            {
                TempData["ErrorMessage"] = "You must be logged in to comment.";
                return RedirectToAction("Login", "Account");
            }
            // Check if comment is empty
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "Comment cannot be empty!";
                return RedirectToAction("Index");
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

            TempData["SuccessMessage"] = "Comment added successfully!";

            return RedirectToAction("Index");
        }
        
        // POST: /BlogPost/DeleteComment/id
        [HttpPost]
        public IActionResult DeleteComment(int id)
        {
            var comment = _db.Comments.Find(id);
            if (comment == null)
            {
                return NotFound();
            }

            // Check if user is authorized to delete the comment
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null || comment.UserId != userId)
            {
                return Unauthorized();
            }

            _db.Comments.Remove(comment);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Comment deleted successfully!";
            return RedirectToAction("Index", "BlogPost");
        }


        [HttpPost]
        public IActionResult EditComment(int id, string content)
        {
            var comment = _db.Comments.FirstOrDefault(c => c.Id == id);
            if (comment == null) return NotFound();

            // Check if user is authorized to edit the comment
            var userId = HttpContext.Session.GetInt32("UserId");
            if (comment.UserId != userId)
            {
                return Unauthorized();
            }

            comment.Content = content;
            comment.UpdatedAt = DateTime.Now;
            _db.SaveChanges();

            // Return the updated comment content in JSON format so it can be used in the view with AJAX
            return Json(new
            {
                content = comment.Content,
                updatedAt = comment.UpdatedAt?.ToString("yyyy-MM-dd HH:mm:ss")
            });
}



    }
}
