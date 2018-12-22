using System.Text;
using System.Threading.Tasks;
using gol.collation.api.Helpers;
using gol.collation.core.Entities;
using gol.collation.core.Models;
using gol.collation.data;
using gol.collation.domain.Interfaces;
using gol.collation.domain.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace gol.collation.api
{
    public class Startup
    {
        private const string TotalAccessCORSPolicy = "TotalAccess";
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

            // Add DbContext service
            services.AddDbContext<CollationContext>(options =>
            {
                options.UseSqlServer(_config.GetConnectionString("CollationDBConnection"));
            });

            //Add Identity Service
            services.AddIdentity<ApiUser, IdentityRole>()
                .AddEntityFrameworkStores<CollationContext>()
                .AddSignInManager<ApiUser>();

            //Add Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidIssuer = _config["Tokens:Issuer"],
                    ValidAudience = _config["Tokens:Audience"],
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"])),
                    ValidateLifetime = true
                };
            });

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
                setupActn.AddPolicy(TotalAccessCORSPolicy, policyConfig =>
                {
                    policyConfig.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(option =>
                {
                    // Avoid serializing circular references of models in JSON
                    option.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                });

            //map Tokens config Section to an object that can be injected in controllers
            var tokensConfig = _config.GetSection("ApiRequestUris");
            //create the injectable servcice
            var conf = services.Configure<TokensConfigSection>(tokensConfig);

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
            app.UseCors(TotalAccessCORSPolicy);
            // Use configured identity service
            app.UseAuthentication();
            //AuthAppBuilderExtensions.UseAuthentication(app);
            app.UseMvc();

            // Seed data for dev environment
            if (env.IsDevelopment())
            {
                //create scope to call DataSeeder
                using (var scope = app.ApplicationServices.CreateScope())
                {
                    //seed database
                    var seeder = scope.ServiceProvider.GetService<DbSeeder>();
                    //since configure is synchronous we cannot use await
                    seeder.SeedData().Wait();
                }
            }
        }
    }
}
