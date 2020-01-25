using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dippy.DDApi {
    public interface IRepository<TEntity> where TEntity : class {
        void Insert(TEntity entity);
        void Insert(TEntity entity, out TEntity inserted);
        void Insert(IEnumerable<TEntity> entities);
        void Insert(IEnumerable<TEntity> entities, out IEnumerable<TEntity> inserted);

        void Update(TEntity entity);
        void Update(IEnumerable<TEntity> entities);

        void Delete(TEntity entity);
        void Delete(IEnumerable<TEntity> entities);

        TEntity Get(TEntity entity);
        IEnumerable<TEntity> Get(IEnumerable<TEntity> entities);

        IEnumerable<TEntity> OffsetPaginate(int pageSize, int offset);
        IEnumerable<TEntity> OffsetPaginate(int pageSize, int offset, string sqlPageOrder);

        IEnumerable<TEntity> KeyedPaginate(int pageSize, TEntity lastKey);
        IEnumerable<TEntity> KeyedPaginate(int pageSize, TEntity lastKey, string sqlPageOrder, string sqlPaginationKey);

        long Count();

        /// <example>
        /// Search(p => p.Foo == 1, && p.Bar <= 25)
        /// </example>
        IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> filter);
        //IEnumerable<TDomainModel> GetAll();
    }
}
