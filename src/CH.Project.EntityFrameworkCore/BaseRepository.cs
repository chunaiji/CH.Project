using CH.Project.EntityFrameworkCore;
using CH.Project.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.Uow;

namespace CH.Project
{
    public class BaseRepository<T> : EfCoreRepository<ProjectDbContext, T>, IBaseRepository<T> where T : class, IEntity, new()
    {
        private IDbContextProvider<ProjectDbContext> _dbContextProvider ;
        protected DbSet<T> _entity;
        protected virtual DbSet<T> Entity => _entity ?? (_entity = _dbContextProvider.GetDbContext().Set<T>());

        public BaseRepository(IDbContextProvider<ProjectDbContext> dbContextProvider)
        : base(dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<int> BaseAdd(T entity)
        {
            await Entity.AddAsync(entity);
            return _dbContextProvider.GetDbContext().SaveChanges();
        }

        public async Task<int> BaseAddRange(List<T> list)
        {
            Entity.AddRange(list);
            return await Task.FromResult(_dbContextProvider.GetDbContext().SaveChanges());
        }

        public async Task<int> BaseRemove(T entity)
        {
            Entity.Remove(entity);
            return await Task.FromResult(_dbContextProvider.GetDbContext().SaveChanges());
        }

        public async Task<int> BaseRemoveRange(List<T> list)
        {
            Entity.RemoveRange(list);
            return await Task.FromResult(_dbContextProvider.GetDbContext().SaveChanges());
        }

        public IQueryable<T> BaseGetAll()
        {
            return Entity.AsQueryable().AsNoTracking();
        }

        public async Task<PageResult<T>> SearchFor<TKey>(int pageIndex, int pageSize, Expression<Func<T, bool>> predicate, bool isAsc, Expression<Func<T, TKey>> keySelector)
        {
            PageResult<T> page = new PageResult<T>();
            return await Task.FromResult(page);
        }

        public async Task<T> GetModelById(object id)
        {
            return await Entity.FindAsync(id);
        }

        public async Task<T> GetModel(Expression<Func<T, bool>> predicate)
        {
            return await Entity.AsQueryable().AsNoTracking().FirstOrDefaultAsync(predicate);
        }

        public async Task<int> BaseCount(Expression<Func<T, bool>> predicate)
        {
            return predicate != null ? await Entity.AsQueryable().AsNoTracking().Where(predicate).CountAsync() : await Entity.CountAsync();
        }

        public async Task<bool> BaseExist(Expression<Func<T, bool>> anyLambda)
        {
            return await Entity.AsQueryable().AsNoTracking().AnyAsync(anyLambda);
        }
    }
}
