using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;

namespace NonHateSpeechForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _context
                .Posts
                .Include(p => p.Author)
                .ToListAsync();

            return User.Identity!.IsAuthenticated ? View(nameof(Index), posts) : View("Guest");
        }
    }
}