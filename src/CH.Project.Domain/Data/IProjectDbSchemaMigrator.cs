using System.Threading.Tasks;

namespace CH.Project.Data
{
    public interface IProjectDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
