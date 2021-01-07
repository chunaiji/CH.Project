using CH.Project.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace CH.Project.Web.Pages
{
    /* Inherit your PageModel classes from this class.
     */
    public abstract class ProjectPageModel : AbpPageModel
    {
        protected ProjectPageModel()
        {
            LocalizationResourceType = typeof(ProjectResource);
        }
    }
}