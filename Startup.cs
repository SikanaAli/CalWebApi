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
using System.Text.Json;
using Quartz;
using System.Collections.Specialized;
using Microsoft.Data.Sqlite;
using CrystalQuartz.AspNetCore;
using CrystalQuartz.Application;
using Quartzmin;


namespace CalWebApi
{
    public class Startup
    {
        private string CalenderSchedulerCorsPolicy = "CalenderSchedulerCorsPolicy";
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
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation()
                .AddJsonOptions(opt=> {
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                    opt.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });
            services.AddApiVersioning(v =>
            {
                v.DefaultApiVersion = new ApiVersion(1, 0);
                v.AssumeDefaultVersionWhenUnspecified = true;
                v.ReportApiVersions = true;
                
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: CalenderSchedulerCorsPolicy, builder =>
                {
                    builder.WithOrigins("https://192.168.10.88:5001");
                    builder.WithOrigins("http://192.168.10.88:5000");
                    builder.AllowAnyOrigin();
                });
            });

            services.AddSwaggerGen(S =>
            {
                S.SchemaFilter<ScheduleTaskModalFilter>();
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                S.IncludeXmlComments(xmlPath);
            });
            

            services.AddSingleton(provider => _QuartzScheduler);

            services.AddHostedService<SchedulerHostedService>();
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
                //ui.InjectJavascript("/js/SwaggerCustomizer.js", "text/javascript");
                ui.InjectStylesheet("/css/SwaggerUI.css");
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
                //{ "quartz.threadPool.type","Quartz.Simpl.SimpleThreadPool, Quartz" },
                //{ "quartz.threadPool.threadCount","Quartz.Simpl.SimpleThreadPool, Quartz" },
                { "quartz.jobStore.type","Quartz.Impl.AdoJobStore.JobStoreTX, Quartz" },
                { "quartz.jobStore.misfireThreshold","60000" },
                { "quartz.jobStore.lockHandler.type","Quartz.Impl.AdoJobStore.UpdateLockRowSemaphore, Quartz" },
                { "quartz.jobStore.useProperties","true" },
                { "quartz.jobStore.tablePrefix","QRTZ_" },
                { "quartz.scheduler.instanceId","Quartz" },
                { "quartz.serializer.type", "json" },
                { "quartz.jobStore.dataSource","default" },
                { "quartz.dataSource.default.provider", "SQLite-Microsoft" },
                { "quartz.jobStore.driverDelegateType","Quartz.Impl.AdoJobStore.SQLiteDelegate, Quartz" },
                { "quartz.dataSource.default.connectionString", "Data Source=Quartz.db3" }
            };

            

            StdSchedulerFactory schedulerFactory = new StdSchedulerFactory(qprops);
            var scheduler = schedulerFactory.GetScheduler().Result;
            
            scheduler.ListenerManager.AddTriggerListener(new TaskTriggerListener());
            return scheduler;
        }
    }
}
