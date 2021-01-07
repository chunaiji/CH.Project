using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Modularity;

namespace CH.Project.EntityFrameworkCore
{
    [DependsOn(
        typeof(ProjectEntityFrameworkCoreModule)
        )]
    public class ProjectEntityFrameworkCoreDbMigrationsModule : AbpModule
    {
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            context.Services.AddAbpDbContext<ProjectMigrationsDbContext>();
        }
    }
}
