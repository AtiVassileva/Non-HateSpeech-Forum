using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML;
using NonHateSpeechForum.Infrastructure;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Controllers
{
    [Authorize]
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

        public async Task<IActionResult> IndexProfanePosts()
        {
            var posts = await _postService.GetProfanePosts();

            if (!User.IsModerator())
            {
                return View("~/Views/Home/Index.cshtml", posts);
            }

            return View("~/Views/Posts/Profinate.cshtml", posts);
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

        public async Task<IActionResult> Approve(Guid id)
        {
            if (!User.IsModerator())
            {
                return Unauthorized();
            }

            try
            {
                await _postService.Approve(id);
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