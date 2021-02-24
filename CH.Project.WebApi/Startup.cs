using CH.Project.Commont.LogCommont;
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
using Newtonsoft.Json;
using Stardust.Monitors;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using XCode.DataAccessLayer;

namespace CH.Project.WebApi
{
    public class Startup
    {
        private string ServerId { get; set; }
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

            Commont.RedisCommont.RedisCommontHelper.CreateInstance().GetRedisClient();//第一次初始化
            Commont.RedisCommont.StackExchangeRedisHelper.redisClient.InitConnect();
            Commont.LogCommont.SerilogActionExtention.CreateInstance();
            #region 星尘分布式监控引用
            var set = Stardust.Setting.Current;
            set.Server = "http://111.230.252.105:6600";
            if (!set.Server.IsNullOrEmpty())
            {
                // APM跟踪器
                var tracer = new StarTracer(set.Server) { Log = XTrace.Log };
                tracer.AppName = "CH.Project_Name显示";
                tracer.AppId = "CH.Project_Appid";
                tracer.AppSecret = "XXXXXXXXXXXXXXXXXXXXX";
                DefaultTracer.Instance = tracer;
                ApiHelper.Tracer = tracer;

                DAL.GlobalTracer = tracer;
                TracerMiddleware.Tracer = tracer;
                Commont.RedisCommont.RedisCommontHelper.CreateInstance().GetRedisClient().Tracer = tracer;
                services.AddSingleton<ITracer>(tracer);
            }
            services.AddControllersWithViews();
            // 引入魔方
            services.AddCube();
            #endregion
            //services.AddSkyApmExtensions();
           

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

            app.InitializeApplication();//非常重要，缺少这个将会导致初始化失败
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();
            app.UseMiddleware<TracerMiddleware>();
            app.UseCube(env);//放比较靠后一点
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");//缺少这个将导致星尘无法发现api接口
            });
     
         

            Subscriber();
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120)
            };
            //webSocketOptions.AllowedOrigins.Add("https://client.com");
            webSocketOptions.AllowedOrigins.Add("*");
            app.UseWebSockets(webSocketOptions);
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        var userId = Guid.NewGuid().ToString();
                        if (context.Request.Query.ContainsKey("userId"))
                        {
                            userId = context.Request.Query["userId"].ToString();
                        }

                        var appServerList = Commont.RedisCommont.StackExchangeRedisHelper.redisClient.GetStringKey<Dictionary<string, string>>("APP-ServerList");
                        appServerList.Add(userId, ServerId);
                        Commont.RedisCommont.StackExchangeRedisHelper.redisClient.SetStringKey("APP-ServerList", JsonConvert.SerializeObject(appServerList));
                        await Commont.WebSocketCommont.WebSocketHelper.InitSocket(userId, webSocket);
                        await Commont.WebSocketCommont.WebSocketHelper.ReceiveAsync(webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });
        }

        private void Subscriber()
        {
            //〇 客户端和服务器端建立连接时需要将自己的身份ID和服务器ID写到数据库，断开连接时删除记录
            //〇 每个服务上线时，根据服务唯一ID在Redis上订阅对应的Topic
            //① 小明发送给小红的消息先被传输到服务A
            //② 服务A收到消息从数据库中查到小红所连接的服务器是服务B
            //③ 服务A将小明的消息转发到Redis中服务B的Topic
            //④ 服务B收到Redis发来的消息
            //⑤ 服务B将消息发送给小红

            var serverList = Commont.RedisCommont.StackExchangeRedisHelper.redisClient.GetStringKey<List<string>>("ServerList");
            if (serverList == null)
            {
                serverList = new List<string>();
            }
            ServerId = Guid.NewGuid().ToString();
            if (!serverList.Contains(ServerId))
            {
                serverList.Add(ServerId);
                foreach (var item in serverList)
                {
                    Commont.RedisCommont.StackExchangeRedisHelper.redisClient.Subscriber($"Server_topic_{item}");
                }
                Commont.RedisCommont.StackExchangeRedisHelper.redisClient.SetStringKey("ServerList", JsonConvert.SerializeObject(serverList));//先用最简单的
            }
        }
    }
}
