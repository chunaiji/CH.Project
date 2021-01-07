using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace CH.Project.Dto
{
    public class OrderMasterDto : EntityDto
    {
        public virtual Guid? TenantId { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public virtual string Amount { get; set; }
        public virtual string Name { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual string UserId { get; set; }

        public virtual List<OrderDetailDto> OrderDetails { get; set; }
    }
}
