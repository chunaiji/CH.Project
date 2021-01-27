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
    public class OrderMasterRepository : BaseRepository<OrderMaster>, IOrderMasterRepository
    {
        private readonly IOrderDetailRepository orderDetails;
        public OrderMasterRepository(IDbContextProvider<ProjectDbContext> dbContextProvider, IServiceProvider serviceProvider)
        : base(dbContextProvider)
        {
            orderDetails = (IOrderDetailRepository)serviceProvider.GetService(typeof(IOrderDetailRepository));
        }

        public async Task<List<OrderMaster>> GetOrderMasterList(string name)
        {
            var list = base.BaseGetAll().Where(u => u.Name.Contains(name)).ToList();
            foreach (var item in list)
            {
                item.OrderDetails = orderDetails.BaseGetAll().Where(u=>u.OrderId==item.Id).ToList();
            }
            return await Task.FromResult(list);
        }

        public async Task<bool> InsertOrderMaster(OrderMaster orderMaster)
        {
            await base.BaseAdd(orderMaster);
            return await Task.FromResult(true);
        }
    }
}