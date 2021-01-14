
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.TenantManagement;

namespace CH.Project
{
    [DependsOn(
        typeof(ProjectDomainModule),
        typeof(AbpAccountApplicationModule),
        typeof(ProjectApplicationContractsModule),
        typeof(AbpIdentityApplicationModule),
        typeof(AbpPermissionManagementApplicationModule),
        typeof(AbpTenantManagementApplicationModule),
        typeof(AbpFeatureManagementApplicationModule)
        )]
    public class ProjectApplicationModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            Configure<AbpAutoMapperOptions>(options =>
            {
                options.AddMaps<ProjectApplicationModule>();
            });

            //Server层注入
            //context.Services.AddScoped<Commont.RedisCommont.IRedisHelper, Commont.RedisCommont.RedisHelper>();
            //Commont.RedisCommont.RedisCommontHelper.CreateInstantiation().GetRedisClient();//第一次初始化
        }
    }
}
