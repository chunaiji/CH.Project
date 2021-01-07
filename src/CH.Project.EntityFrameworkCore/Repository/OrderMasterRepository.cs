using CH.Project.Demo;
using CH.Project.EntityFrameworkCore;
using CH.Project.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace CH.Project.Repository
{
    public class OrderMasterRepository : EfCoreRepository<ProjectDbContext, OrderMaster>, IOrderMasterRepository
    {
        public OrderMasterRepository(IDbContextProvider<ProjectDbContext> dbContextProvider)
        : base(dbContextProvider)
        {

        }

        public async Task<List<OrderMaster>> GetOrderMasterList(string name)
        {
            var list = DbSet.AsQueryable().Where(u => u.Name.Contains(name)).ToList();
            return await Task.FromResult(list);
        }

        public async Task<bool> InsertOrderMaster(OrderMaster orderMaster)
        {
            DbSet.Add(orderMaster);
            await DbContext.SaveChangesAsync();
            return await Task.FromResult(true);
        }
    }
}