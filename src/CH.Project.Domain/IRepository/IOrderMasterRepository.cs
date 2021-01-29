using CH.Project.Demo;
using CH.Project.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CH.Project.IRepository
{
    public interface IOrderMasterRepository : IBaseRepository<OrderMaster>
    {
        Task<List<OrderMaster>> GetOrderMasterList(OrderMasterRequest request);
        Task<bool> InsertOrderMaster(OrderMaster orderMaster);
    }
}
