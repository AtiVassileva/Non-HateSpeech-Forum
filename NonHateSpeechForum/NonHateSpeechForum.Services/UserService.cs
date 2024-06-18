using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Services.Contracts;
using NonHateSpeechForum.Services.Response;

namespace NonHateSpeechForum.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserResponseModel>> GetAll()
        {
            var users = await _context
                .Users
                .ToListAsync();

            var userResponseModels = new List<UserResponseModel>();

            foreach (var user in users)
            {
                userResponseModels.Add(new UserResponseModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    CurrentRole = await GetCurrentRole(user.Id)
                });
            }

            return userResponseModels;
        }

        private async Task<string> GetCurrentRole(string userId)
        {
            var userRole = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);

            if (userRole == null)
            {
                return "Regular User";
            }

            var role = await _context.Roles
                .FirstAsync(r => r.Id == userRole.RoleId);

            return role.Name;
        }
    }
}