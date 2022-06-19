﻿using System;
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
using ChristmasPi.Hardware;
using ChristmasPi.Data.Models.Scheduler;
using ChristmasPi.Util.Wrappers;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace ChristmasPi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            HandleWaitForPid();
            ConfigureTree();
            ConfigurationManager.Instance.InitializeShutdown();
            ConfigurationManager.Instance.InitializeLogger();
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
            services.Configure<MvcOptions>(options => {
                options.EnableEndpointRouting = false;
            });
            // Use JSON.Net from https://thecodebuzz.com/add-newtonsoft-json-support-net-core/
            services.AddControllersWithViews().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
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

            app.UseSerilogRequestLogging();

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
            ConfigurationManager.Instance.LoadConfiguration();
            ConfigurationManager.Instance.LoadSchedule();
            ConfigurationManager.Instance.Configuration = Configuration;   
        }

        /// <summary>
        /// Handles waiting for a PID to exit if required
        /// </summary>
        public void HandleWaitForPid() {
            if (ConfigurationManager.Instance.RuntimeConfiguration.DaemonMode) {
                // check for the existence of a pid file
                if (PIDFile.Load() is int pid) {
                    PIDFile.Consume();
                    Log.ForContext<Startup>().Debug("Waiting up to 30 seconds for PID {pid} to exit", pid);
                    // start waiting for pid to exit
                    bool pidexists = false;
                    for (int i = 0; i < Constants.REBOOT_MAX_ATTEMPTS; i++) {
                        if (!pidexistswrapper.PidExists(pid)) {
                            pidexists = false;
                            break;
                        }
                        pidexists = true;
                        if (i == Constants.REBOOT_MAX_ATTEMPTS - 1)
                            Log.ForContext<Startup>().Error("Timedout waiting for old process to exit");
                        Thread.Sleep(Constants.REBOOT_POLL_SLEEP);
                    }
                    if (!pidexists)
                        Log.ForContext<Startup>().Debug("Successfully rebooted process");
                }
            }
        }
        /// <summary>
        /// Starts tree services
        /// </summary>
        public void StartTree() {
            AnimationManager.Instance.Init();
            OperationManager.Instance.Init();
            HardwareManager.Instance.Init(true);
            Controllers.RedirectHandler.Init();
            Log.Debug("Starting tree");
        }
    }
}