using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace CH.Project.Demo
{
    public class OrderDetail : AuditedEntity<string>
    {
        public virtual Guid? TenantId { get; set; }

        public virtual string OrderId { get; set; }

        public virtual string GoodsNo { get; set; }
        public virtual string GoodsName { get; set; }
        public void SetId()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
