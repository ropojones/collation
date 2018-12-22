using gol.collation.core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gol.collation.data
{
    public class DbSeeder
    {
        private const string SEED_USER_ERROR = "Failed to create seed user";
        private const string SEED_USERNAME = "testuser";
        private readonly UserManager<ApiUser> _userManager;
        private readonly RoleManager<ApiUser> _rolemanager;
        private readonly CollationContext _ctx;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(UserManager<ApiUser> userManager, 
            RoleManager<ApiUser> roleManager,
            CollationContext ctx,
            ILogger<DbSeeder> logger)
        {
            _userManager = userManager;
            _rolemanager = roleManager;
            _ctx = ctx;
            _logger = logger;
        }

        public async Task SeedData()
        {
            await _ctx.Database.EnsureCreatedAsync();

            //check if user exist before creating user
            var userSeed = await _userManager.FindByNameAsync(SEED_USERNAME);
            if (userSeed == null)
            {
                userSeed = new ApiUser
                {
                    UserName = SEED_USERNAME,
                    Email = "testuser@test.com",
                };

                var result = await _userManager.CreateAsync(userSeed, "P@ssword1!");
                if (result != IdentityResult.Success)
                {
                    _logger.LogError(SEED_USER_ERROR);
                    throw new InvalidOperationException(SEED_USER_ERROR);
                }
            }
        }
    }
}
