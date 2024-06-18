using Microsoft.AspNetCore.Mvc;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;

        public HomeController(IPostService postService)
        {
            _postService = postService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAll();
            return User.Identity!.IsAuthenticated ? View(nameof(Index), posts) : View("Guest");
        }
    }
}