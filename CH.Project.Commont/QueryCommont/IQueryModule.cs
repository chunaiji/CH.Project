//using System;
//using System.Collections.Generic;
//using System.Linq.Expressions;
//using System.Text;

//namespace CH.Project.Commont.QueryCommont
//{
//    public interface IQueryModule<TEntity> where TEntity : class
//    {
//        Expression<Func<TEntity, bool>> GenerateExpression();
//    }

//    public class Query<TEntity> : IQueryModule<TEntity> where TEntity : class
//    {
//        /// <summary>
//        /// 指定查询条件
//        /// </summary>
//        protected Expression<Func<TEntity, bool>> Predicate;

//        /// <summary>
//        /// 创建一个新的 <see cref="Query{TEntity}"/>
//        /// </summary>
//        public Query()
//        {

//        }

//        /// <summary>
//        /// 创建一个指定查询条件的<see cref="Query{TEntity}"/>
//        /// </summary>
//        /// <param name="predicate">指定的查询条件</param>
//        public Query(Expression<Func<TEntity, bool>> predicate) : this()
//        {
//            Predicate = predicate;
//        }

//        /// <summary>
//        /// 生成表达式
//        /// </summary>
//        /// <returns></returns>
//        public Expression<Func<TEntity, bool>> GenerateExpression()
//        {
//            return this.GenerateExpressionCore();
//        }
//    }
//}
