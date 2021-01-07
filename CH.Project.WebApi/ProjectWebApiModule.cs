using CH.Project.EntityFrameworkCore;
using CH.Project.Localization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;

namespace CH.Project.WebApi
{
    [DependsOn(
           typeof(ProjectHttpApiModule),
           typeof(ProjectDomainModule),
           typeof(ProjectApplicationModule),
           typeof(ProjectEntityFrameworkCoreModule),
         typeof(ProjectApplicationContractsModule),
          typeof(ProjectDomainSharedModule),
           typeof(AbpAspNetCoreMvcModule),
           typeof(AbpAutofacModule)
           )]
    public class ProjectWebApiModule : AbpModule
    {
        public override void PreConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
            {
                options.AddAssemblyResource(
                    typeof(ProjectResource),
                    typeof(ProjectDomainModule).Assembly,
                    typeof(ProjectDomainSharedModule).Assembly,
                    typeof(ProjectApplicationModule).Assembly,
                    typeof(ProjectEntityFrameworkCoreModule).Assembly,
                    typeof(ProjectApplicationContractsModule).Assembly
                );
            });
        }

        public override void OnApplicationInitialization(ApplicationInitializationContext context)
        {
            var app = context.GetApplicationBuilder();
            var env = context.GetEnvironment();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
        }
    }
}
