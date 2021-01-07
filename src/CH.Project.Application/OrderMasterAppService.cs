using CH.Project.Demo;
using CH.Project.Dto;
using CH.Project.IRepository;
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
        private readonly IOrderDetailRepository _orderDetailRepository;
        public OrderMasterAppService(IOrderMasterRepository orderMasterRepository, IOrderDetailRepository orderDetailRepository)
        {
            _orderMasterRepository = orderMasterRepository;
            _orderDetailRepository = orderDetailRepository;
        }

        public async Task<List<OrderMasterDto>> GetOrderMasterList(string name)
        {
            var list = await _orderMasterRepository.GetOrderMasterList(name);
            foreach (var item in list)
            {
                item.OrderDetails = _orderDetailRepository.AsQueryable().Where(u => u.OrderId == item.Id).ToList();
            }
            return ObjectMapper.Map<List<OrderMaster>, List<OrderMasterDto>>(list);
        }

        public async Task<bool> InsertOrderMaster(string name)
        {
            OrderMaster entity = new OrderMaster() { Name = name };
            entity.SetId();
            var flag = await _orderMasterRepository.InsertOrderMaster(entity);
            if (flag)
            {
                var orderDetail = new OrderDetail() { OrderId = entity.Id, GoodsName = "测试", GoodsNo = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") };
                orderDetail.SetId();
                await _orderDetailRepository.InsertAsync(orderDetail);
            }
            return flag;
        }
    }
}
