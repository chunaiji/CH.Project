using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CH.Project.Commont.QueryCommont
{
    public interface IQueryModule<TEntity> where TEntity : class
    {
        Expression<Func<TEntity, bool>> GenerateExpression();
    }
}
