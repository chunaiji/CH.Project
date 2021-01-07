using Volo.Abp.Settings;

namespace CH.Project.Settings
{
    public class ProjectSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            //Define your own settings here. Example:
            //context.Add(new SettingDefinition(ProjectSettings.MySetting1));
        }
    }
}
