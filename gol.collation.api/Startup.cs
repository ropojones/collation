using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gol.collation.data;
using gol.collation.data.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace gol.collation.api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<CollationContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("CollationDBConnection"));
            });

            //Add Identity Service
            services.AddIdentity<ApiUser, IdentityRole>().AddEntityFrameworkStores<CollationContext>();

            // Setup Identity Functionality
            services.Configure<IdentityOptions>();

            // Add cross origin support to specify API access levels from third parties
            services.AddCors(setupActn =>
            {
                setupActn.AddPolicy("TotalAccess", policyConfig =>
                {
                    policyConfig.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
                });
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
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
