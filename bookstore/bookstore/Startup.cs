using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using bookstore.DataAccess.AppContext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using bookstore.Entities;
using Microsoft.AspNetCore.Identity;

namespace bookstore
{
    public class Startup
    {
        private readonly IConfiguration _config;
        public Startup(IConfiguration  configuration)
        {
            _config = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(cfg => cfg.UseSqlServer(_config.GetConnectionString("DefaultConnection")));
            services.AddIdentity<AppUser, IdentityRole>(cfg =>
                                                        {
                                                            cfg.User.RequireUniqueEmail = true;
                                                        }).AddEntityFrameworkStores<DataContext>();
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();   // UseAuthorization and UseAuthentication below UseRoute middleware
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                                 "default",
                                  "{controller}/{action}/{id?}",
                                  new {controller ="Home", action ="Index" });
            });
        }
    }
}
