using System;
using System.Collections.Generic;
using System.Text;

namespace CH.Project.QueryCommont
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
        public string[] PropertyPaths { get; set; }

        public string PropertyPath { get; set; }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryModeAttribute(params string[] propertyPath)
        {
            PropertyPaths = propertyPath;
        }

        /// <summary>
        /// 查询字段
        /// </summary>
        public QueryModeAttribute(QueryCompare compare, params string[] propertyPath) : this(propertyPath)
        {
            Compare = compare;
        }
    }
}
