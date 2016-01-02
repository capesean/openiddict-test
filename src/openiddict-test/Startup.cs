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
            // add the config file which stores the connection string
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            // add entity framework using the config connection string
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            // add identity: note the AddOpenIddictCore call
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddOpenIddictCore<Application>(config => config.UseEntityFramework());

            // assuming you have an api...
            services.AddMvc();

            // for seeding the database with the demo user details
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        }

        public void Configure(IApplicationBuilder app, IDatabaseInitializer databaseInitializer)
        {
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            // to serve up index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // don't use identity as this is a wrapper for using cookies, not needed
            //app.UseIdentity();

            app.UseOpenIddictCore(builder =>
            {
                // tell openiddict you're wanting to use jwt tokens
                builder.Options.UseJwtTokens();
                // NOTE: for dev consumption only! for live, this is not encouraged!
                builder.Options.AllowInsecureHttp = true;
                builder.Options.ApplicationCanDisplayErrors = true;
            });

            // this enables the validation of the api using oauth (i.e. using the access/bearer token)
            app.UseOAuthValidation(options =>
            {
                options.AutomaticAuthenticate = true;
                options.AutomaticChallenge = true;
            });

            // assuming you have an api...
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            // seed the database
            databaseInitializer.Seed();
        }
    }
}
