using CH.Project.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace CH.Project.Controllers
{
    /* Inherit your controllers from this class.
     */
    public abstract class ProjectController : AbpController
    {
        protected ProjectController()
        {
            LocalizationResource = typeof(ProjectResource);
        }
    }
}