using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace openiddicttest
{
    public static class Program
    {
        public static void Main(string[] args)
        {
			var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();


            var host = new WebHostBuilder()
                .ConfigureLogging(options => options.AddConsole())
                .ConfigureLogging(options => options.AddDebug())
                .UseConfiguration(configuration)
                .UseIISIntegration()
                .UseKestrel()
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
