using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.Commont.QueryCommont
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class QueryModeAttribute : Attribute
    {
        /// <summary>
        /// 比较方式
        /// </summary>
        public QueryCompare Compare { get; set; }

        /// <summary>
        /// 对应属性路径
        /// </summary>
        public string[] PropertyPath { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryModeAttribute(params string[] propertyPath)
        {
            PropertyPath = propertyPath;
        }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryModeAttribute(QueryCompare compare, params string[] propertyPath)
        {
            PropertyPath = propertyPath;
            Compare = compare;
        }
    }
}
