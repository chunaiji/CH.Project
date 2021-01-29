using CH.Project.Demo;
using CH.Project.QueryCommont;
using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Request
{
    public class OrderMasterRequest : Query<OrderMaster>
    {
        [QueryMode(Compare = QueryCompare.Equal, PropertyPath = nameof(OrderMaster.Name))]
        public virtual string Name { get; set; }

        [QueryMode(Compare = QueryCompare.LessThanOrEqual, PropertyPath = nameof(OrderMaster.Age))]
        public virtual int Age { get; set; }

        [QueryMode(Compare = QueryCompare.GreaterThanOrEqual, PropertyPath = nameof(OrderMaster.Amount))]
        public virtual decimal Amount { get; set; }
    }
}
