using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Models;
using openiddicttest.Models;
using System.Linq;

namespace openiddicttest
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public static void Main(string[] args)
        {
            var application = new WebApplicationBuilder()
                .UseConfiguration(WebApplicationConfiguration.GetDefault(args))
                .UseStartup<Startup>()
                .Build();

            application.Run();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddOpenIddictCore<Application>(config => config.UseEntityFramework());

            services.AddMvc();

            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        }

        public void Configure(IApplicationBuilder app, IDatabaseInitializer databaseInitializer)
        {
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseDefaultFiles();
            app.UseStaticFiles();

            // don't use identity as this is a wrapper for using cookies
            //app.UseIdentity();

            app.UseOpenIddictCore(builder =>
            {
                builder.Options.UseJwtTokens();
                // for dev
                // NOTE: for live, this is not encouraged!
                builder.Options.AllowInsecureHttp = true;
                builder.Options.ApplicationCanDisplayErrors = true;
            });

            app.UseOAuthValidation(options =>
            {
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            databaseInitializer.Seed();
        }
    }
}
