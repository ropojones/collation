using System.Threading.Tasks;
using gol.collation.core.Entities;
using gol.collation.core.Models;
using gol.collation.data;
using gol.collation.domain.Interfaces;
using gol.collation.domain.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace gol.collation.api
{
    public class Startup
    {
        public readonly IConfiguration _config;
        private readonly ILogger<Startup> _logger;

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            _config = configuration;
            _logger = logger;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //add the configuration service
            services.AddSingleton(_config);

            services.AddDbContext<CollationContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("CollationDBConnection"));
            });

            //Add Identity Service
            services.AddIdentity<ApiUser, IdentityRole>().AddEntityFrameworkStores<CollationContext>().AddSignInManager<ApiUser>();

            // Setup Identity Functionality
            services.ConfigureApplicationCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents()
                {
                    // If request is api and request is not authenticated return 401
                    OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                            ctx.Response.StatusCode = 401;
                        return Task.CompletedTask;
                    },

                    // If request is api and user doesn't have acces to method return 403
                    OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                            ctx.Response.StatusCode = 403;
                        return Task.CompletedTask;
                    }
                };
            });

            // Add cross origin support to specify API access levels from third parties
            services.AddCors(setupActn =>
            {
                setupActn.AddPolicy("TotalAccess", policyConfig =>
                {
                    policyConfig.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            //Add business specific services
            services.AddScoped<IAuthService, AuthService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }
            // Use configured identity service
            AuthAppBuilderExtensions.UseAuthentication(app);
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
