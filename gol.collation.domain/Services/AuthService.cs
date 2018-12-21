using gol.collation.core.Entities;
using gol.collation.core.Models;
using gol.collation.core.RequestModels;
using gol.collation.domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gol.collation.domain.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApiUser> _signInManager;

        public AuthService(SignInManager<ApiUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task<bool> LoginWithPasswordAsync(PasswordLoginRequest loginRequest)
        {
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password,false, false);
            if (result.Succeeded)
                return true;
            return false;
        }
    }
}
