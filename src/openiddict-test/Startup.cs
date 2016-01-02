using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenIddict;
using OpenIddict.Models;
using openiddicttest.Models;
using System.Linq;
using CryptoHelper;

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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up configuration sources.

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json");
            //.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            //if (env.IsDevelopment())
            //{
            //    // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
            //    //builder.AddUserSecrets();

            //    // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
            //    //builder.AddApplicationInsightsSettings(developerMode: true);
            //}

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
            // Add framework services.
            //services.AddApplicationInsightsTelemetry(Configuration);

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddOpenIddict();

            services.AddMvc();

            // Add application services.
            //services.AddTransient<IEmailSender, AuthMessageSender>();
            //services.AddTransient<ISmsSender, AuthMessageSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            //loggerFactory.AddDebug();

            //app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                //app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            //app.UseApplicationInsightsExceptionTelemetry();

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseIdentity();

            app.UseOpenIddict(builder =>
            {
                builder.Options.AllowInsecureHttp = true;
                builder.Options.ApplicationCanDisplayErrors = true;
            });

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            using (var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>())
            {
                context.Database.EnsureCreated();

                // Add Mvc.Client to the known applications.
                if (context.Applications.Any())
                {
                    foreach (var application in context.Applications)
                        context.Remove(application);
                    context.SaveChanges();
                }
                // Note: when using the introspection middleware, your resource server
                // MUST be registered as an OAuth2 client and have valid credentials.
                // 
                // context.Applications.Add(new Application {
                //     Id = "resource_server",
                //     DisplayName = "Main resource server",
                //     Secret = "875sqd4s5d748z78z7ds1ff8zz8814ff88ed8ea4z4zzd"
                // });

                context.Applications.Add(new Application
                {
                    Id = "openiddict-test",
                    DisplayName = "My test application",
                    RedirectUri = "http://localhost:58292/signin-oidc",
                    LogoutRedirectUri = "http://localhost:58292/",
                    Secret = Crypto.HashPassword("secret_secret_secret"),
                    Type = OpenIddictConstants.ApplicationTypes.Public
                });

                context.SaveChanges();
            }
        }
    }
}
