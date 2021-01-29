﻿using CH.Project.Demo;
using CH.Project.QueryCommont;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CH.Project.Request
{
    public class RequestData : Query<OrderMaster>
    {
        [QueryMode(Compare = QueryCompare.Equal, PropertyPath = nameof(OrderMaster.Name))]
        public virtual string Name { get; set; }

        [QueryMode(Compare = QueryCompare.LessThanOrEqual, PropertyPath = nameof(OrderMaster.Age))]
        public virtual int Age { get; set; }

        [QueryMode(Compare = QueryCompare.GreaterThanOrEqual, PropertyPath = nameof(OrderMaster.Amount))]
        public virtual decimal Amount { get; set; }
    }
}
