using CH.Project.Demo;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace CH.Project.EntityFrameworkCore
{
    public static class ProjectDbContextModelCreatingExtensions
    {
        public static void ConfigureProject(this ModelBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            /* Configure your own tables/entities inside here */

            builder.Entity<OrderMaster>(b =>
            {
                b.ToTable("OrderMaster");
                b.ConfigureByConvention(); //auto configure for the base class props
            });
            builder.Entity<OrderDetail>(b =>
            {
                b.ToTable("OrderDetail");
                //b.ToTable(ProjectConsts.DbTablePrefix + "YourEntities", ProjectConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
            });
        }
    }
}