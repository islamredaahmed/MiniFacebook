using FecebookAPI.Models;

namespace FecebookAPI.Contracts
{
    public interface IAuth
    {
        Task<AuthModel> RegisterAsync(RegisterModel model);
        Task<AuthModel> UserOTPAsync(OTPUserModel model);
        Task<AuthModel> ResendOTPAsync(string mobile);
        Task<AuthModel> GetTokenAsync(TokenRequestModel model);
        Task<AuthModel> RefreshTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        UserModel GetCurrentUser();
    }
}
