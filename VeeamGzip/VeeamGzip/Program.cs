using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO;
using VeeamGzip.Interfaces;
using VeeamGzip.Services;

namespace VeeamGzip
{
    public class Program
    {
        public static void Main()
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build())
                .Enrich.FromLogContext()
                .WriteTo.File($"logs/log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            // Configure services
            var host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IConsoleApplication, ConsoleApplication>();
                    services.AddTransient<ICompressible, Compress>();
                    services.AddTransient<IDecompressible, Decompress>();
                })
                .UseSerilog()
                .Build();
            
            var services = ActivatorUtilities.CreateInstance<ConsoleApplication>(host.Services);
            services.Run();
        }

        private static void BuildConfig(IConfigurationBuilder builder)
        {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}
