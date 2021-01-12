using CH.Project.WebApi.Attributes;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using NewLife.Cube;
using NewLife.Cube.WebMiddleware;
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
            services.AddMvc(op => op.EnableEndpointRouting = false).AddRazorPagesOptions(o =>
                {
                    o.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
                });

            #region �ǳ��ֲ�ʽ�������
            var set = Stardust.Setting.Current;
            set.Server = "http://111.230.252.105:6600";
            if (!set.Server.IsNullOrEmpty())
            {
                // APM������
                var tracer = new StarTracer(set.Server) { Log = XTrace.Log };
                tracer.AppName = "CH.Project_Name��ʾ";
                tracer.AppId = "CH.Project_Appid";
                tracer.AppSecret = "XXXXXXXXXXXXXXXXXXXXX";
                DefaultTracer.Instance = tracer;
                ApiHelper.Tracer = tracer;

                DAL.GlobalTracer = tracer;
                TracerMiddleware.Tracer = tracer;
                services.AddSingleton<ITracer>(tracer);
            }
            services.AddControllersWithViews();
            // ����ħ��
            services.AddCube();
            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var set = Setting.Current;
            if (env.IsDevelopment() || set.Debug)
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CH.Project.WebApi v1"));
            }
            else
            {
                app.UseHsts();
            }

            app.InitializeApplication();//�ǳ���Ҫ��ȱ��������ᵼ�³�ʼ��ʧ��
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<TracerMiddleware>();
            app.UseCube(env);//�űȽϿ���һ��
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");//ȱ������������ǳ��޷�����api�ӿ�
            });
        }
    }
}
