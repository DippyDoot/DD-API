using System.Collections.Generic;

namespace Dippy.DDApi {
    public interface IEntityRepository<TEntity> : IDomainModelRepository<TEntity> where TEntity : class {
        void Insert(TEntity entity, out long id);
        void Insert(IEnumerable<TEntity> entities, out IEnumerable<long> ids);

        void Delete(long id);
        void Delete(IEnumerable<long> id);

        TEntity Get(long id);
        IEnumerable<TEntity> Get(IEnumerable<long> id);

        IEnumerable<long> OffsetPaginateId(int pageSize, int offset);
        IEnumerable<long> OffsetPaginateId(int pageSize, int offset, string sqlPageOrder);

        IEnumerable<TEntity> KeyedPaginate(int pageSize, long lastId);
        IEnumerable<TEntity> KeyedPaginate(int pageSize, long lastId, string sqlPageOrder, string sqlPaginationKey);

        IEnumerable<long> KeyedPaginateId(int pageSize, long lastId);
        IEnumerable<long> KeyedPaginateId(int pageSize, long lastId, string sqlPageOrder, string sqlPaginationKey);
    }
}
