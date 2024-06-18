using Microsoft.EntityFrameworkCore;
using NonHateSpeechForum.Data;
using NonHateSpeechForum.Data.Models;
using NonHateSpeechForum.Services.Contracts;
using Microsoft.Extensions.ML;
using Microsoft.ML;

namespace NonHateSpeechForum.Services
{
    using static Common.ErrorMessages;

    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;
        private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

        public PostService(ApplicationDbContext context, PredictionEnginePool<ModelInput, ModelOutput> predictionEngine)
        {
            _context = context;

            // Load the trained model
            var mlContext = new MLContext();
            var modelPath = "C:\\Users\\atanasia.vasileva\\OneDrive - Scale Focus AD\\Desktop\\Non-HateSpeech-Forum\\NonHateSpeechForum\\profanity_detection_model.zip";
            ITransformer trainedModel;
            using (var stream = new FileStream(modelPath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                trainedModel = mlContext.Model.Load(stream, out _);
            }

            // Create prediction engine
            _predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(trainedModel);

        }

        public async Task<IEnumerable<Post>> GetAll()
        {
            var posts = await _context
                .Posts
                .Include(p => p.Author)
                .Where(p => !p.IsFlagged)
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<Post>> GetProfanePosts()
        {
            var posts = await _context.Posts
                .Include(p => p.Author)
                .Where(p => p.IsFlagged)
                .ToListAsync();

            return posts;
        }

        public async Task<bool> Create(string authorId, string content)
        {
            var isProfane = ContainsProfanity(content);

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

        public async Task<bool> Approve(Guid id)
        {
            var post = await FindPost(id);
            post!.IsFlagged = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> Delete(Guid id)
        {
            var post = await FindPost(id);

            _context.Posts.Remove(post!);
            await _context.SaveChangesAsync();
            return true;
        }

        private bool ContainsProfanity(string content)
        {
            var input = new ModelInput { Text = content };
            var prediction = _predictionEngine.Predict(input);
            return prediction.IsProfane;
        }
        private async Task<Post?> FindPost(Guid id)
        {
            var post = await _context
                .Posts
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null)
            {
                throw new NullReferenceException(NonExistingPostErrorMessage);
            }

            return post;
        }
    }
}