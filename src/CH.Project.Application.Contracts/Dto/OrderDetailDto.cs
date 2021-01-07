using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace CH.Project.Dto
{
    public class OrderDetailDto : EntityDto
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string OrderId { get; set; }

        public virtual string GoodsNo { get; set; }
        public virtual string GoodsName { get; set; }
    }
}
