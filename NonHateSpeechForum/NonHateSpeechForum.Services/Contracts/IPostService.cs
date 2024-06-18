using NonHateSpeechForum.Data.Models;

namespace NonHateSpeechForum.Services.Contracts
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAll();
        Task<IEnumerable<Post>> GetProfanePosts();
        Task<bool> Create(string userId, string content);
        Task<bool> Approve(Guid id);
        Task<bool> Delete(Guid id);
    }
}