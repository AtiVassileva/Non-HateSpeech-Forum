using Microsoft.AspNetCore.Mvc;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly IPostService _postService;
        private readonly IUserService _userService;

        public HomeController(IPostService postService, IUserService userService)
        {
            _postService = postService;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAll();
            return User.Identity!.IsAuthenticated ? View(nameof(Index), posts) : View("Guest");
        }

        public async Task<IActionResult> Admin()
        {
            var users = await _userService.GetAll();
            return View(users);
        }
    }
}