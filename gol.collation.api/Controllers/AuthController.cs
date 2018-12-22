using gol.collation.domain.Interfaces;
using gol.collation.core.RequestModels;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gol.collation.api.Filters;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using gol.collation.api.Helpers;

namespace gol.collation.api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("TotalAccess")]
    [ValidateModel]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;
        private readonly TokensConfigSection _tokensConfig;

        public AuthController(IAuthService authService, 
            ILogger<AuthController> logger,
            TokensConfigSection tokensConfig)
        {
            _authService = authService;
            _logger = logger;
            _tokensConfig = tokensConfig;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var isloggedIn = _authService.LoginAsync(request);
                if (isloggedIn.Result)
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while logging in");
            }
            return BadRequest("Failed to login");
        }

        [HttpPost("token")]
        public IActionResult CreateToken([FromBody] LoginRequest request)
        {
            try
            {
                var user = _authService.AuthenticateApiUserAsync(request).Result;
                if (user != null)
                {
                    var userClaims = _authService.GetClaimsAsync(user).Result;
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    }.Union(userClaims);

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokensConfig.Key));
                    var cred = new SigningCredentials(key, SecurityAlgorithms.Aes128CbcHmacSha256);

                    var token = new JwtSecurityToken(
                        issuer: _tokensConfig.Issuer,
                        audience: _tokensConfig.Audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(60),
                        signingCredentials: cred
                        );

                    return Ok(new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while generating JWT");
            }
            return BadRequest("Failed to generate token")
        }
    }
}
