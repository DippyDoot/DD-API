using System.Collections.Generic;

namespace Dippy.DDApi {
    public interface IEntityRepository<TEntity> : IDomainModelRepository<TEntity> where TEntity : class {
        void Insert(TEntity entity, out long id);
        void Insert(IEnumerable<TEntity> entities, out IEnumerable<long> ids);

        void Delete(long id);
        void Delete(IEnumerable<long> id);

        TEntity Get(long id);
        IEnumerable<TEntity> Get(IEnumerable<long> id);
    }
}
