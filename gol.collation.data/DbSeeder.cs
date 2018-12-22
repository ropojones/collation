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
    public static class DbSeeder
    {
        private const string SEED_USER_ERROR = "Failed to create seed user";
        private const string SEED_USERNAME = "testuser";

        public static void SeedData(CollationContext ctx,
            UserManager<ApiUser> userManager)
        {
            ctx.Database.EnsureCreated();

            if (userManager.FindByNameAsync(SEED_USERNAME).Result == null)
            {
                var userSeed = new ApiUser
                {
                    UserName = SEED_USERNAME,
                    Email = "testuser@test.com",
                };

                var result = userManager.CreateAsync(userSeed, "P@ssword1!").Result;
                if (result != IdentityResult.Success)
                {
                    throw new InvalidOperationException(SEED_USER_ERROR);
                }
            }
        }
    }
}
