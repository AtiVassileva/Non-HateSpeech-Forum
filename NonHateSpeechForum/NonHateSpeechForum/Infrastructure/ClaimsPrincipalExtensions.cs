using System.Security.Claims;

namespace NonHateSpeechForum.Infrastructure
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetId(this ClaimsPrincipal user)
            => user.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        public static bool IsModerator(this ClaimsPrincipal user)
            => user.IsInRole("Moderator");

        public static bool IsAdmin(this ClaimsPrincipal user)
            => user.IsInRole("Administrator");
    }
}