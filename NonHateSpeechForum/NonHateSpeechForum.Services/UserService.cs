using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<IdentityUser>> GetAll()
        {
            var users = await _context
                .Users
                .ToListAsync();

            return users;
        }
    }
}