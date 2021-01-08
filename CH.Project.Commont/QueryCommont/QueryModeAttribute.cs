using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.QueryCommont
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class QueryModeAttribute : Attribute
    {
    }
}
