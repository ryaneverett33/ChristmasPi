using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ChristmasPi.Data;
using ChristmasPi.Data.Models;
using ChristmasPi.Operations;
using ChristmasPi.Animation;
using System.IO;
using Microsoft.Extensions.Hosting;

namespace ChristmasPi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            ConfigureTree();
            StartTree();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_3_0);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        /// <summary>
        /// Loads the tree and animation configurations
        /// </summary>
        public void ConfigureTree() {
            // Load tree configuration
            if (!File.Exists("configuration.json")) {
                Console.WriteLine("LOGTHIS Tree Configuration file not found, using default configuration values");
                ConfigurationManager.Instance.StartupTreeConfig = TreeConfiguration.DefaultSettings();
            }
            else {
                string json = File.ReadAllText("configuration.json");
                ConfigurationManager.Instance.StartupTreeConfig = JsonConvert.DeserializeObject<TreeConfiguration>(json);
            }
            ConfigurationManager.Instance.CurrentTreeConfig = ConfigurationManager.Instance.StartupTreeConfig;
            ConfigurationManager.Instance.Configuration = Configuration;
        }
        /// <summary>
        /// Starts tree services
        /// </summary>
        public void StartTree() {
            OperationManager.Instance.Init();
            AnimationManager.Instance.Init();
        }
    }
}
