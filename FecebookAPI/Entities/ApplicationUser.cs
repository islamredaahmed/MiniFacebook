using Microsoft.AspNetCore.Identity;

namespace FecebookAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public List<RefreshToken>? RefreshTokens { get; set; }
        public List<OTPUser>? OTPUsers { get; set; }
        public virtual List<Post> Posts { get; set; }
        public virtual List<UserFriend> UserFriends { get; set; }
    }
}
