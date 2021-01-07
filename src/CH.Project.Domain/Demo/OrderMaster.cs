using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace CH.Project.Demo
{
    public class OrderMaster : AuditedEntity<string>
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

        public virtual List<OrderDetail> OrderDetails { get; set; }

        public void SetId()
        {
            this.Id = Guid.NewGuid().ToString();
        }
    }
}
