using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace CH.Project.Web
{
    [Dependency(ReplaceServices = true)]
    public class ProjectBrandingProvider : DefaultBrandingProvider
    {
        public override string AppName => "Project";
    }
}
