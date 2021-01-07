using CH.Project.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace CH.Project.IRepository
{
    public interface IOrderMasterRepository : IRepository<OrderMaster>,ITransientDependency
    {
        Task<List<OrderMaster>> GetOrderMasterList(string amount);
        Task<bool> InsertOrderMaster(OrderMaster orderMaster);
    }
}
