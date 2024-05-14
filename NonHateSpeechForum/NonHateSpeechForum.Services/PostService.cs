using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Data.Models;
using NonHateSpeechForum.Services.Contracts;
using Microsoft.Extensions.ML;

namespace NonHateSpeechForum.Services
{
    using static Common.ErrorMessages;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly PredictionEnginePool<ModelInput, ModelOutput> _predictionEngine;

        public PostService(ApplicationDbContext context, PredictionEnginePool<ModelInput, ModelOutput> predictionEngine)
        {
            _context = context;
            _predictionEngine = predictionEngine;
        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            var posts = await _context
                .Posts
                .Include(p => p.Author)
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetProfanePosts()
        {
            var posts = await _context.Posts.Where(p=>p.IsFlagged).ToListAsync();
            return posts;
        }
        
        public async Task<bool> Create(string authorId, string content)
        {
            bool isProfane = ContainsProfanity(content);
            var newPost = new Post
            {
                AuthorId = authorId,
                Content = content,
                IsFlagged = isProfane
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

        private bool ContainsProfanity(string content)
        {
            var input = new ModelInput { Content = content };
            var prediction = _predictionEngine.Predict("ProfanityModel", input);
            return prediction.IsProfane;
        }
    }
}