using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
            return this.expression.And(this.GenerateExpression());
        }

       
    }
}
