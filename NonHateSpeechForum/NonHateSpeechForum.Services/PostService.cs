using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Data.Models;
using NonHateSpeechForum.Services.Contracts;

namespace NonHateSpeechForum.Services
{
    using static Common.ErrorMessages;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            var posts = await _context
                .Posts
                .Include(p => p.Author)
                .ToListAsync();

            return posts;
        }

        public async Task<bool> Create(string authorId, string content)
        {
            var newPost = new Post
            {
                AuthorId = authorId,
                Content = content
            };

            await _context.Posts.AddAsync(newPost);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var post = await _context
                .Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                throw new NullReferenceException(NonExistingPostErrorMessage);
            }

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}