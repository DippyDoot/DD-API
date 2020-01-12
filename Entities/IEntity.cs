using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.Entities {
    public interface IEntity<TEntity> where TEntity : class {
        long Id { get; }
    }
}
