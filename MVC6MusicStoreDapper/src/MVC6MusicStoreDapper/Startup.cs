namespace MVC6MusicStoreDapper
{
    using Microsoft.AspNet.Builder;
    using Microsoft.AspNet.Hosting;
    using Microsoft.Data.Entity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.PlatformAbstractions;

    using MVC6MusicStore.Core.DAL;
    using MVC6MusicStore.Core.DAL.ADO.NET;
    using MVC6MusicStore.Core.Services;

    using MVC6MusicStoreDapper.DataContext;

    using Serilog;

    using ILogger = Serilog.ILogger;

    public class Startup
    {
        private IConfiguration Configuration { get; }

        public Startup(IApplicationEnvironment env)
        {
            var builder = new ConfigurationBuilder().SetBasePath(env.ApplicationBasePath).AddJsonFile("config.json").AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF services to the services container
            services.AddEntityFramework()
                         .AddSqlServer()
                         .AddDbContext<MusicStoreContext>(options =>
                             options.UseSqlServer(this.Configuration["Data:DefaultConnection:ConnectionString"]));

            services.Configure<DefaultConfiguration>(options =>
            {
                options.ConnectionString = this.Configuration["Data:DefaultConnection:ConnectionString"];
            });
            
            services.AddInstance<IConfiguration>(this.Configuration);
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IDatabaseAdapter, DatabaseAdapter>();
            services.AddScoped<IStoreAdoNetRepository, StoreAdoNetRepository>();

            services.AddMvc();

            // Logging
            services.AddLogging();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            loggerfactory.AddSerilog(GetLoggerConfiguration());

            app.UseDatabaseErrorPage()
                .UseStaticFiles()
                .UseMvc(routes =>
                {
                    routes.MapRoute(
                        name: "default",
                        template: "{controller}/{action}/{id?}",
                        defaults: new { controller = "Home", action = "Index" });
                });

            // Populates the MusicStore sample data
            SampleData.InitializeMusicStoreDatabaseAsync(app.ApplicationServices).Wait();
        }

        private static ILogger GetLoggerConfiguration()
        {
            return new LoggerConfiguration()
                        .MinimumLevel.Debug()
                        .WriteTo.RollingFile(@"C:\Temp\DapperMVC6-{Date}.txt", outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level}:{EventId} [{SourceContext}] {Message}{NewLine}{Exception}")
                        .WriteTo.ColoredConsole()
                        .CreateLogger();
        }
    }
}
