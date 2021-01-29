using CH.Project.Commont.RedisCommont;
using CH.Project.Demo;
using CH.Project.Dto;
using CH.Project.IRepository;
using CH.Project.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace CH.Project
{
    public class OrderMasterAppService : ApplicationService, IOrderMasterAppService
    {
        private readonly IOrderMasterRepository _orderMasterRepository;
        public OrderMasterAppService(IOrderMasterRepository orderMasterRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderMasterRepository = orderMasterRepository;
        }

        public async Task<List<OrderMasterDto>> GetOrderMasterList(OrderMasterRequest request)
        {
            var key = "CHProject:Demo:" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + request.Name;
            if (RedisCommontHelper.CreateInstance().ContainsKey(key))
            {
                var entity = RedisCommontHelper.CreateInstance().Get<OrderMaster>(key);
                return ObjectMapper.Map<List<OrderMaster>, List<OrderMasterDto>>(new List<OrderMaster>() { entity });
            }
            var list = await _orderMasterRepository.GetOrderMasterList(request);
            return ObjectMapper.Map<List<OrderMaster>, List<OrderMasterDto>>(list);
        }

        public async Task<bool> InsertOrderMaster(string name)
        {
            var key = "CHProject:Demo:" + DateTime.Now.ToString("yyyy-MM-dd") + ":" + name;
            OrderMaster entity = new OrderMaster() { Name = name };
            entity.SetId();
            entity.OrderDetails = new List<OrderDetail>();
            var orderDetail = new OrderDetail() { OrderId = entity.Id, GoodsName = "测试", GoodsNo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
            orderDetail.SetId();
            entity.OrderDetails.Add(orderDetail);
            var flag = await _orderMasterRepository.InsertOrderMaster(entity);
            if (flag)
            {
                if (!RedisCommontHelper.CreateInstance().ContainsKey(key))
                {
                    RedisCommontHelper.CreateInstance().Set<OrderMaster>(key, entity);
                }
            }
            return flag;
        }
    }
}
