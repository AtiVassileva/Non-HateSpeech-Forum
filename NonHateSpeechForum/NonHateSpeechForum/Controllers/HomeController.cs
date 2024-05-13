using Microsoft.AspNetCore.Mvc;

namespace NonHateSpeechForum.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return User.Identity!.IsAuthenticated ? View(nameof(Index)) : View("Guest");
        }
    }
}