using CH.Project.Dto;
using CH.Project.Request;
using CH.Project.WebApi.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace CH.Project.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [ServiceFilter(typeof(GlobalExceptionFilter))]
    [IgnoreAntiforgeryToken]
    public class OrderController : ControllerBase, ITransientDependency
    {
        private readonly IOrderMasterAppService _orderMasterAppService;
        public OrderController(IOrderMasterAppService orderMasterAppService)
        {
            _orderMasterAppService = orderMasterAppService;
        }


        [HttpPost]
        public async Task<bool> InsertOrder([FromBody]RequestData  requestData)
        {
            return await _orderMasterAppService.InsertOrderMaster(requestData.name);
        }

        [HttpGet]
        public async Task<List<OrderMasterDto>> GetOrderMasterList(string name)
        {
            return await _orderMasterAppService.GetOrderMasterList(name);
        }
    }


}
