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

namespace gol.collation.api.Controllers
{
    [Route("api/[controller]")]
    [EnableCors("TotalAccess")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("/login")]
        public IActionResult Login([FromBody] PasswordLoginRequest request)
        {
            try
            {
                var isloggedIn = _authService.LoginWithPassword(request);
                if (isloggedIn.Result)
                    return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while logging in");
            }
            return BadRequest("Failed to login");
        }
    }
}
