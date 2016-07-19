using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // add identity
            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
				.AddUserManager<CustomOpenIddictManager>()
                .AddDefaultTokenProviders();

            // add OpenIddict
            services.AddOpenIddict<ApplicationUser, ApplicationRole, ApplicationDbContext>()
                .DisableHttpsRequirement()
                .EnableTokenEndpoint("/connect/token")
                .AllowPasswordFlow()
                .AllowRefreshTokenFlow()
                .UseJsonWebTokens();

            // assuming you have an api...
            services.AddMvc();

            // for seeding the database with the demo user details
            services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
        }

        public void Configure(IApplicationBuilder app, IDatabaseInitializer databaseInitializer)
        {
            app.UseDeveloperExceptionPage();

            // to serve up index.html
            app.UseDefaultFiles();
            app.UseStaticFiles();

            // don't use identity as this is a wrapper for using cookies, not needed
            //app.UseIdentity();

            app.UseOpenIddict();

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
