using CH.Project.Demo;
using CH.Project.EntityFrameworkCore;
using CH.Project.IRepository;
using CH.Project.Request;
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

        public async Task<List<OrderMaster>> GetOrderMasterList(OrderMasterRequest request)
        {
            var list = base.SearchFor(request.GenerateExpression()).ToList();
            foreach (var item in list)
            {
                item.OrderDetails = orderDetails.BaseGetAll().Where(u=>u.OrderId==item.Id).ToList();
            }
            return await Task.FromResult(list);
        }

        public async Task<bool> InsertOrderMaster(OrderMaster orderMaster)
        {
            var result = await base.BaseAdd(orderMaster);
            if (result > 0)
            {
                foreach (var item in orderMaster.OrderDetails)
                {
                    await orderDetails.BaseAdd(item);
                }
            }
            return await Task.FromResult(result > 0);
        }
    }
}