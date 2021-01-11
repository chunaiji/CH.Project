using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CH.Project.Commont.QueryCommont
{
    public class QueryModule<TEntity> : IQueryModule<TEntity> where TEntity : class
    {
        public virtual Expression<Func<TEntity, bool>> expression { get; set; }
        public QueryModule()
        {

        }

        public QueryModule(Expression<Func<TEntity, bool>> expression) : this()
        {
            this.expression = expression;
        }

        public virtual Expression<Func<TEntity, bool>> GenerateExpression()
        {
            //return Expression<Func<TEntity, bool>>.Lambda(expression.Body,).Add(expression, this.GenerateQueryExpressionCore());
            return null;
        }

       
    }
}
