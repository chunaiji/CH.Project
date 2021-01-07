using CH.Project.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace CH.Project
{
    [DependsOn(
        typeof(ProjectEntityFrameworkCoreTestModule)
        )]
    public class ProjectDomainTestModule : AbpModule
    {

    }
}