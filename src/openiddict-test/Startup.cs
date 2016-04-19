using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict;
using OpenIddict.Models;
using openiddicttest.Models;

namespace openiddicttest
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // add the config file which stores the connection string
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            // add entity framework using the config connection string
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            // add identity: note the AddOpenIddictCore call
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddOpenIddictCore<Application>(config => config.UseEntityFramework());

            // assuming you have an api...
            services.AddMvc();

            // for seeding the database with the demo user details
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            services.AddScoped<OpenIddictManager<ApplicationUser, Application>, CustomOpenIddictManager>();
        }

        public void Configure(IApplicationBuilder app, IDatabaseInitializer databaseInitializer)
        {
            app.UseDeveloperExceptionPage();

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

            // use jwt bearer authentication
            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                RequireHttpsMetadata = false,
                Audience = "http://localhost:58292/",
                Authority = "http://localhost:58292/"
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
