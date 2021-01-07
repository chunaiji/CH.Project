using CH.Project.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace CH.Project
{
    public interface IOrderMasterAppService : IApplicationService, ITransientDependency
    {
        Task<List<OrderMasterDto>> GetOrderMasterList(string name);

        Task<bool> InsertOrderMaster(string name);
    }
}
