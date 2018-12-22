using gol.collation.core.Entities;
using gol.collation.core.RequestModels;
using gol.collation.domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gol.collation.domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApiUser> _signInManager;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IPasswordHasher<ApiUser> _hasher;

        public AuthService(SignInManager<ApiUser> signInManager, 
            UserManager<ApiUser> userManager,
            IPasswordHasher<ApiUser> hasher)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _hasher = hasher;
        }

        public async Task<bool> LoginAsync(LoginRequest loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password,false, false);
            if (result.Succeeded)
                return true;
            return false;
        }

        public async Task<ApiUser> AuthenticateApiUserAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user != null)
            {
                if (_hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Success)
                    return user;
            }
            return null;
        }

        public async Task<IList<Claim>> GetClaimsAsync(ApiUser user) => await _userManager.GetClaimsAsync(user);
    }
}
