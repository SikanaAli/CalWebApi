using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation;
using Microsoft.AspNetCore.Mvc.Versioning;
using Quartz.Impl;
using CalWebApi.Scheduler;
using Microsoft.AspNetCore.Mvc;
using CalWebApi.Filters;
using System.Reflection;
using System.IO;
using Quartz;
using System.Collections.Specialized;

namespace CalWebApi
{
    public class Startup
    {
        private IScheduler _QuartzScheduler;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _QuartzScheduler = ConfigurQuartz();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
                v.ReportApiVersions = true;
            });
            services.AddSwaggerGen(S =>
            {
                S.SchemaFilter<ScheduleTaskModalFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                S.IncludeXmlComments(xmlPath);
            });
            var scheduler = StdSchedulerFactory.GetDefaultScheduler().GetAwaiter().GetResult();

            services.AddSingleton(provider => _QuartzScheduler);
            //services.AddHostedService<SchedulerHostedService>();
        }

        private void OnShutdown()
        {
            //shutdown quratz if its not already shutdown
            if (!_QuartzScheduler.IsShutdown) _QuartzScheduler.Shutdown();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            //app.UseHttpsRedirection();
            app.UseStaticFiles();


            app.UseSwagger(S =>
            {
                S.SerializeAsV2 = true;
            });

            app.UseSwaggerUI(ui =>
            {
                ui.SwaggerEndpoint("/swagger/v1/swagger.json", "Cal API");
                ui.RoutePrefix = "Doc";

            });

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        public IScheduler ConfigurQuartz()
        {
            NameValueCollection qprops = new NameValueCollection()
            {
                { "quartz.serializer.type", "binary" },
            };

            StdSchedulerFactory schedulerFactory = new StdSchedulerFactory(qprops);
            var scheduler = schedulerFactory.GetScheduler().Result;
            scheduler.Start().Wait();
            scheduler.ListenerManager.AddTriggerListener(new TaskTriggerListener());
            return scheduler;
        }
    }
}
