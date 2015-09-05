using System;
using ManytoMany.Dal;
using Microsoft.AspNet.Builder;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Dnx.Runtime;

namespace ManytoMany
{
    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IApplicationEnvironment env)
        {
            var builder = new ConfigurationBuilder(env.ApplicationBasePath).AddJsonFile("config.json").AddEnvironmentVariables(); 
            this.Configuration = builder.Build();
        }
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF services to the services container.
            services.AddEntityFramework()
                         .AddSqlServer()
                         .AddDbContext<StoreDbContext>(options =>
                             options.UseSqlServer(this.Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            ////app.UseWelcomePage();

            app.UseErrorPage()
              .UseStaticFiles()
              ////.UseIdentity()
              .UseMvc(routes =>
              {
                  routes.MapRoute(
                      name: "default",
                      template: "{controller}/{action}/{id?}",
                      defaults: new { controller = "Home", action = "Index" });
              });

            // Populates the Admin user and role 
            SampleData.InitializeIdentityDatabaseAsync(app.ApplicationServices).Wait();
        }
    }
}
