using CH.Project.Demo;
using CH.Project.EntityFrameworkCore;
using CH.Project.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace CH.Project.Repository
{
    public class OrderDetailRepository : BaseRepository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(IDbContextProvider<ProjectDbContext> dbContextProvider)
        : base(dbContextProvider)
        {

        }
    }
}
