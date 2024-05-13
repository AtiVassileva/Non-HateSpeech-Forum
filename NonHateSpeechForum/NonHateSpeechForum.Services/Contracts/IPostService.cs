using NonHateSpeechForum.Data.Models;

namespace NonHateSpeechForum.Services.Contracts
{
    public interface IPostService
    {
        Task<IEnumerable<Post>> GetAll();
        Task<bool> Create(string userId, string content);
        Task<bool> Delete(Guid id);
    }
}