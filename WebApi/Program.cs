using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Serilog;
using System.Net;
using Serilog.Events;
using System.IO;
using System;

namespace GrafanaProxy.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File("/logs/grafana-proxy-.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, 80);
                    var useHttps = false;
                    var useHttpsStr = Environment.GetEnvironmentVariable("GRAFANA_PROXY_USE_HTTPS");
                    if (!string.IsNullOrEmpty(useHttpsStr))
                        useHttpsStr = useHttpsStr.ToLower();
                    if (bool.TryParse(useHttpsStr, out useHttps) && useHttps)
                    {
                        options.Listen(IPAddress.Any, 443, listenOptions =>
                        {
                            {
                                var casheeCertFileName = Environment.GetEnvironmentVariable("GRAFANA_PROXY_PFX_FILENAME");
                                var casheeCertFilePass = Environment.GetEnvironmentVariable("GRAFANA_PROXY_PFX_PASS");

                                var certPath = Path.Combine(Directory.GetCurrentDirectory(), casheeCertFileName);
                                Log.Logger.Debug("Try to get ssl certificate from path '{@certPath}'", certPath);
                                listenOptions.UseHttps(certPath, casheeCertFilePass);
                            }
                        });
                    }                    
                })
                .Build();            
    }    
}
