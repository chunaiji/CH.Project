using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace CH.Project.IRepository
{
    public interface IBaseRepository<TEntity> : IRepository<TEntity>, ITransientDependency where TEntity : class, IEntity, new()
    {
        /// <summary>
        /// 添加实体(单个)
        /// </summary>
        /// <param name="entity">实体对象</param>
        Task<int> BaseAdd(TEntity entity);

        /// <summary>
        /// 批量插入实体(多个)
        /// </summary>
        /// <param name="list">实体列表</param>
        Task<int> BaseAddRange(List<TEntity> list);

        /// <summary>
        /// 删除实体(单个)
        /// </summary>
        /// <param name="entity"></param>
        Task<int> BaseRemove(TEntity entity);

        /// <summary>
        /// 批量删除实体(多个)
        /// </summary>
        /// <param name="list">实体列表</param>
        Task<int> BaseRemoveRange(List<TEntity> list);
        /// <summary>
        /// 获取所有
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> BaseGetAll();
        /// <summary>
        /// 分页条件查询
        /// </summary>
        /// <typeparam name="TKey">排序类型</typeparam>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页大小</param>
        /// <param name="predicate">条件表达式</param>
        /// <param name="isAsc">是否升序排列</param>
        /// <param name="keySelector">排序表达式</param>
        /// <returns></returns>
        Task<PageResult<TEntity>> SearchFor<TKey>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> predicate,
            bool isAsc, Expression<Func<TEntity, TKey>> keySelector);
        /// <summary>
        /// 获取实体（主键）
        /// </summary>
        /// <param name="id">主键id</param>
        /// <returns></returns>
        Task<TEntity> GetModelById(object id);
        /// <summary>
        /// 获取实体（条件）
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns></returns>
        Task<TEntity> GetModel(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 查询记录数
        /// </summary>
        /// <param name="predicate">条件表达式</param>
        /// <returns>记录数</returns>
        Task<int> BaseCount(Expression<Func<TEntity, bool>> predicate);
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="anyLambda">查询表达式</param>
        /// <returns>布尔值</returns>
        Task<bool> BaseExist(Expression<Func<TEntity, bool>> anyLambda);
    }

    public class PageResult<T> where T : class
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// 集合总数
        /// </summary>
        public int TotalRows { get; set; }
        /// <summary>
        /// 每页项数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 集合
        /// </summary>
        public IList<T> LsList { get; set; }
    }
}
