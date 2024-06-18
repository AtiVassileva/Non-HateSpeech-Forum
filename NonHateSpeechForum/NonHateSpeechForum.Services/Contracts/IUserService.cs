using Microsoft.AspNetCore.Identity;

namespace NonHateSpeechForum.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<IdentityUser>> GetAll();
    }
}