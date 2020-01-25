﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Dippy.DDApi {
    public interface IRepository<TDomainModel> where TDomainModel : class {
        void Insert(TDomainModel model);
        void Insert(IEnumerable<TDomainModel> models);

        void Update(TDomainModel model);
        void Update(IEnumerable<TDomainModel> models);

        void Delete(TDomainModel model);
        void Delete(IEnumerable<TDomainModel> models);

        TDomainModel Get(TDomainModel model);
        IEnumerable<TDomainModel> Get(IEnumerable<TDomainModel> models);

        IEnumerable<TDomainModel> OffsetPaginate(int pageSize, int offset);
        IEnumerable<TDomainModel> OffsetPaginate(int pageSize, int offset, string sqlPageOrder);

        IEnumerable<TDomainModel> KeyedPaginate(int pageSize, TDomainModel lastKey);
        IEnumerable<TDomainModel> KeyedPaginate(int pageSize, TDomainModel lastKey, string sqlPageOrder, string sqlPaginationKey);

        long Count();

        /// <example>
        /// Search(p => p.Foo == 1, && p.Bar <= 25)
        /// </example>
        IEnumerable<TDomainModel> Search(Expression<Func<TDomainModel, bool>> filter);
        //IEnumerable<TDomainModel> GetAll();
    }
}
