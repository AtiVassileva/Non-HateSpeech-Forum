using NonHateSpeechForum.Services.Response;

namespace NonHateSpeechForum.Services.Contracts
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponseModel>> GetAll();
        Task<bool> MakeRegularUser(string userId);
        Task<bool> MakeModerator(string userId);
        Task<bool> MakeAdministrator(string userId);
    }
}