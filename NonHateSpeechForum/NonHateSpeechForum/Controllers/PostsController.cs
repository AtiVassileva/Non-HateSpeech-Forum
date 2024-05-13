using Microsoft.AspNetCore.Mvc;
using NonHateSpeechForum.Infrastructure;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Controllers
{
    public class PostsController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IPostService _postService;

        public PostsController(IPostService postService, ILogger<PostsController> logger)
        {
            _postService = postService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var posts = await _postService.GetAll();
            return View("~/Views/Home/Index.cshtml", posts);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string content)
        {
            try
            {
                await _postService.Create(User.GetId(), content);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred: {e.Message}");
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _postService.Delete(id);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred: {e.Message}");
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}