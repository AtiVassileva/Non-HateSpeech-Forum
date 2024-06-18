using NonHateSpeechForum.Services.Response;

namespace NonHateSpeechForum.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseModel>> GetAll();
    }
}