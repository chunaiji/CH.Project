using Volo.Abp.Modularity;

namespace CH.Project
{
    [DependsOn(
        typeof(ProjectApplicationModule),
        typeof(ProjectDomainTestModule)
        )]
    public class ProjectApplicationTestModule : AbpModule
    {

    }
}