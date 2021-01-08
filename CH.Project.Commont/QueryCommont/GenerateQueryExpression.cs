using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CH.Project.Commont.QueryCommont
{
    public static class GenerateQueryExpression
    {
        public static Expression<Func<TEntity, bool>> GenerateExpression<TEntity>(this IQueryModule<TEntity> queryModule) where TEntity : class
        {
            if (queryModule == null)
            {
                return null;
            }
            var type = queryModule.GetType();
            var param = Expression.Parameter(typeof(TEntity), "m");
            Expression body = null;
            foreach (var property in type.GetProperties())
            {
                var value = property.GetValue(queryModule);
                if (value is string)
                {
                    value = string.IsNullOrEmpty((string)value) ? string.Empty : value.ToString().Trim();
                }
                Expression sub = null;
                foreach (var item in property.GetAttributes<QueryModeAttribute>())
                {

                }
            }
            return null;



        }
    }
}
