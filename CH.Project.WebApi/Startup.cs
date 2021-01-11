using CH.Project.WebApi.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NewLife.Cube;
using NewLife.Log;
using NewLife.Remoting;
using Stardust.Monitors;
using System;
using XCode.DataAccessLayer;

namespace CH.Project.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "CH.Project.WebApi", Version = "v1" });
            });
            services.AddScoped<GlobalExceptionFilter>();
            services.AddHttpContextAccessor();
            services.AddApplication<ProjectWebApiModule>();
            services.AddMvc(op=>op.EnableEndpointRouting=false).AddRazorPagesOptions(o =>
            {
                o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
            });

            #region 星链分布式监控引用
            var set = Stardust.Setting.Current;
            set.Server = "http://111.230.252.105:6600";
            if (!set.Server.IsNullOrEmpty())
            {
                // APM跟踪器
                var tracer = new StarTracer(set.Server) { Log = XTrace.Log };
                tracer.AppName = "CH.Project";
                DefaultTracer.Instance = tracer;
                ApiHelper.Tracer = tracer;
                DAL.GlobalTracer = tracer;
                NewLife.Cube.WebMiddleware.TracerMiddleware.Tracer = tracer;
                services.AddSingleton<ITracer>(tracer);
            }
            services.AddControllersWithViews();
            // 引入魔方
            services.AddCube();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CH.Project.WebApi v1"));
            }

            app.UseMvc(builder =>
            {
                builder.MapRoute("Default", "{controller=Home}/{action=Index}/{id?}");
            });

          

            app.InitializeApplication();//非常重要，缺少这个将会导致初始化失败
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseCube(env);//放比较靠后一点
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
