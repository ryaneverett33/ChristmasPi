using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ChristmasPi.Data;
using Serilog;

namespace ChristmasPi
{
    public class Server
    {
        public static void Main(string[] args)
        {
            if (!ConfigurationManager.Instance.LoadDebugConfiguration(args)) {
                Console.WriteLine("Debug Configuration failed to load, exiting");
                return;
            }
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog()
                .ConfigureKestrel(options => {
                    options.ListenAnyIP(50808);
                })
                .UseKestrel();
    }
}