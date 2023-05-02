using FecebookAPI.Contracts;
using FecebookAPI.Entities;
using FecebookAPI.ExceptionHandler;
using FecebookAPI.Models;
using FecebookAPI.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace FecebookAPI.Services
{
    public class AuthService : IAuth
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IStringLocalizer<SharedResource> localizer, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _localizer = localizer;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<AuthModel> RegisterAsync(RegisterModel model)
        {
            if (await _userManager.FindByNameAsync(model.Username) is not null 
                || _userManager.Users.Any(x => x.PhoneNumber == model.MobileNumber))
               throw new CustomAuthorizationException(string.Format(_localizer["Username or mobile number already registered!"]));

            var user = new ApplicationUser
            {
                UserName = model.Username,
                PhoneNumber = model.MobileNumber,
                PhoneNumberConfirmed = false
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;

                foreach (var error in result.Errors)
                    errors += $"{error.Description},";

                return new AuthModel { Message = errors };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var otp = GenerateOTP();
            user.OTPUsers?.Add(otp);
            await _userManager.UpdateAsync(user);

            return new AuthModel { Message = "success" }; 
        }

        public async Task<AuthModel> ResendOTPAsync(string mobile)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == mobile);
            if (user is null)
                throw new CustomAuthorizationException(string.Format(_localizer["Mobile Number not Fount"]));

            var otp = GenerateOTP();
            user.OTPUsers?.Add(otp);
            await _userManager.UpdateAsync(user);

            return new AuthModel { Message = "success" };
        }

        public async Task<AuthModel> UserOTPAsync(OTPUserModel model)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == model.MobileNumber);
            if(user is null)
                throw new CustomAuthorizationException(string.Format(_localizer["Mobile Number not Fount"]));

            var userOTP = user.OTPUsers?.FirstOrDefault(i => i.IsActive == true);

            if(userOTP is null)
                throw new CustomAuthorizationException(string.Format(_localizer["Generate new OTP"]));

            var jwtSecurityToken = await CreateJwtToken(user);
            var refreshToken = GenerateRefreshToken();
            user.RefreshTokens?.Add(refreshToken);

            userOTP.RevokedOn = DateTime.UtcNow;
            user.PhoneNumberConfirmed = true;
            await _userManager.UpdateAsync(user);
            return new AuthModel
            {
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                RefreshToken = refreshToken.Token,
                RefreshTokenExpiration = refreshToken.ExpiresOn
            };
        }

        public async Task<AuthModel> GetTokenAsync(TokenRequestModel model)
        {
            var authModel = new AuthModel();

            var user = await _userManager.FindByNameAsync(model.Username);

            if (!user.PhoneNumberConfirmed)
                throw new CustomAuthorizationException(string.Format(_localizer["You must verify account"]));

            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
                throw new CustomAuthorizationException(string.Format(_localizer["Username or Password is incorrect!"]));

            var jwtSecurityToken = await CreateJwtToken(user);
            var rolesList = await _userManager.GetRolesAsync(user);

            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

            if (user.RefreshTokens.Any(t => t.IsActive))
            {
                var activeRefreshToken = user.RefreshTokens.FirstOrDefault(t => t.IsActive);
                authModel.RefreshToken = activeRefreshToken.Token;
                authModel.RefreshTokenExpiration = activeRefreshToken.ExpiresOn;
            }
            else
            {
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.ExpiresOn;
                user.RefreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        private async Task<JwtSecurityToken> CreateJwtToken(ApplicationUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = new List<Claim>();

            foreach (var role in roles)
                roleClaims.Add(new Claim("roles", role));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", user.Id)
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<AuthModel> RefreshTokenAsync(string token)
        {
            var authModel = new AuthModel();

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
            throw new CustomAuthorizationException(string.Format(_localizer["Invalid token"]));

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
            {
                authModel.Message = "Inactive token";
                return authModel;
            }

            refreshToken.RevokedOn = DateTime.UtcNow;

            var newRefreshToken = GenerateRefreshToken();
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            authModel.IsAuthenticated = true;
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            var roles = await _userManager.GetRolesAsync(user);
            authModel.RefreshToken = newRefreshToken.Token;
            authModel.RefreshTokenExpiration = newRefreshToken.ExpiresOn;

            return authModel;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.RefreshTokens.Any(t => t.Token == token));

            if (user == null)
                return false;

            var refreshToken = user.RefreshTokens.Single(t => t.Token == token);

            if (!refreshToken.IsActive)
                return false;

            refreshToken.RevokedOn = DateTime.UtcNow;

            await _userManager.UpdateAsync(user);

            return true;
        }

        private RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];

            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                Token = Convert.ToBase64String(randomNumber),
                ExpiresOn = DateTime.UtcNow.AddDays(1),
                CreatedOn = DateTime.UtcNow
            };
        }
        private OTPUser GenerateOTP()
        {
            Random random = new Random();
            var otp = random.Next(999999);

            return new OTPUser
            {
                OTP = otp.ToString(),
                ExpiresOn = DateTime.UtcNow.AddMinutes(2),
                CreatedOn = DateTime.UtcNow
            };
        }

        public UserModel GetCurrentUser()
        {
            UserModel user = new UserModel();
            user.Username = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "";
            user.UserId = _httpContextAccessor.HttpContext?.User.FindFirstValue("uid") ?? "";
            user.Role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role) ?? "";
            return user;
        }

    }
}
