using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Data.Models;
using NonHateSpeechForum.Infrastructure;

namespace NonHateSpeechForum.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context
                .Posts
                .Include(p => p.Author)
                .ToListAsync();

            return View("~/Views/Home/Index.cshtml", posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content)
        {
            var authorId = User.GetId();
            var newPost = new Post
            {
                AuthorId = authorId,
                Content = content
            };

            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}