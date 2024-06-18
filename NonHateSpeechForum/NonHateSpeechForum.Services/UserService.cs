using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Services.Contracts;
using NonHateSpeechForum.Services.Response;

namespace NonHateSpeechForum.Services
{
    using static Common.ErrorMessages;

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

        public async Task<bool> MakeRegularUser(string userId)
        {
            var regularUserId = await GetRoleId("Regular User");
            await UpdateRole(userId, regularUserId);
            return true;
        }

        public async Task<bool> MakeModerator(string userId)
        {
            var moderatorId = await GetRoleId("Moderator");
            await UpdateRole(userId, moderatorId);
            return true;
        }

        public async Task<bool> MakeAdministrator(string userId)
        {
            var administratorId = await GetRoleId("Administrator");
            await UpdateRole(userId, administratorId);
            return true;
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

        private async Task<string> GetRoleId(string roleName)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == roleName.ToLower());

            if (role == null)
            {
                throw new ArgumentNullException(NonExistingRoleErrorMessage);
            }

            return role.Id;
        }

        private async Task UpdateRole(string userId, string desiredRoleId)
        {
            var currentUserRole = await _context.UserRoles
                .FirstAsync(ur => ur.UserId == userId);

            _context.UserRoles.Remove(currentUserRole);
            await _context.SaveChangesAsync();

            await _context.UserRoles.AddAsync(new IdentityUserRole<string>
            {
                UserId = userId, RoleId = desiredRoleId
            });
            await _context.SaveChangesAsync();
        }
    }
}