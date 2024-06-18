using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly ILogger<PostsController> _logger;
        private readonly IUserService _userService;

        public UsersController(IUserService userService, ILogger<PostsController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAll();
            return View(users);
        }

        public async Task<IActionResult> MakeRegularUser(string userId)
        {
            try
            {
                await _userService.MakeRegularUser(userId);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred: {e.Message}");
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> MakeModerator(string userId)
        {
            try
            {
                await _userService.MakeModerator(userId);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, $"An unexpected error occurred: {e.Message}");
                return BadRequest();
            }

            return RedirectToAction(nameof(Index));
        }
        
        public async Task<IActionResult> MakeAdmin(string userId)
        {
            try
            {
                await _userService.MakeAdministrator(userId);
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