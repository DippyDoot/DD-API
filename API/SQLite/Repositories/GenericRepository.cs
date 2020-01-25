using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Dapper;
using ExpressionExtensionSQL.Dapper;
using Dippy.DDApi.Attributes;

namespace Dippy.DDApi.SQLite.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        

        protected string TableName { get; } // MyTable
        protected string DefaultPageOrder { get; } //used in ORDER BY -- "Foo, Bar"
                                                   //used in ORDER BY -- "Foo ASC, Bar DESC"
        protected string DefaultKeyedPaginationKey { get; } // used in WHERE -- "(Foo, Bar) > (@Foo, @Bar)"
                                                            // used in WHERE -- "Foo > @Foo"
                                                            // is default because need to flip '>' for desc order
        protected string SqlInsertByKey { get; } 
        protected string SqlUpdateByKey { get; }
        protected string SqlDeleteByKey { get; }
        protected string SqlGetByKey { get; }
        protected string SqlGetLastInserted { get; }
        protected string SqlCount { get; }
        
        protected Func<SQLiteConnection> DbConnectionFactory { get; }

        public GenericRepository(Func<SQLiteConnection> dbConnectionFactory, RepoInitInfo info)
        {
            if (dbConnectionFactory == null)        throw new ArgumentNullException(nameof(dbConnectionFactory));
            if (info == null)                       throw new ArgumentNullException(nameof(info));

            DbConnectionFactory = dbConnectionFactory;
            
            DefaultPageOrder = info.OrderByKey;
            DefaultKeyedPaginationKey = info.DefaultKeyedPaginationKey;

            TableName = info.TableName;

            SqlInsertByKey = info.SqlInsertByKey;
            SqlUpdateByKey = info.SqlUpdateByKey;
            SqlDeleteByKey = info.SqlDeleteByKey;
            SqlGetByKey = info.SqlGetByKey;
            SqlGetLastInserted = info.SqlGetLastInserted;
            SqlCount = info.SqlGetByKey;
        }

        #region IDomainModelRepository
        public void Insert(TEntity entity)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlInsertByKey, entity);
            }
        }
        public void Insert(TEntity entity, out TEntity inserted)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        cnn.Execute(SqlInsertByKey, entity, transaction: transaction);
                        inserted = cnn.QueryFirst<TEntity>(SqlGetLastInserted, transaction: transaction);
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void Insert(IEnumerable<TEntity> entities) 
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TEntity entity in entities)
                        {
                            cnn.Execute(SqlInsertByKey, entity, transaction: transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        public void Insert(IEnumerable<TEntity> entities, out IEnumerable<TEntity> inserted)
        {
            var output = new List<TEntity>();

            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TEntity entity in entities)
                        {
                            cnn.Execute(SqlInsertByKey, entity, transaction: transaction);
                            output.Add(cnn.QueryFirst<TEntity>(SqlGetLastInserted, transaction: transaction));
                        }
                        transaction.Commit();
                        inserted = output;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Update(TEntity entity)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlUpdateByKey, entity);
            }
        }
        public void Update(IEnumerable<TEntity> entities)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TEntity entity in entities)
                        {
                            cnn.Execute(SqlUpdateByKey, entity, transaction: transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public void Delete(TEntity entity)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlDeleteByKey, entity);
            }
        }
        public void Delete(IEnumerable<TEntity> entities)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TEntity entity in entities)
                        {
                            cnn.Execute(SqlDeleteByKey, entity, transaction: transaction);
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public TEntity Get(TEntity entity)
        {
            using (var cnn = GetNewConnection())
            {
                TEntity newModel = cnn.QueryFirst<TEntity>(SqlGetByKey, entity);
                return newModel;
            }
        }
        public IEnumerable<TEntity> Get(IEnumerable<TEntity> entities)
        {
            var newModels = new List<TEntity>();
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TEntity entity in entities)
                        {
                            newModels.Add(cnn.QueryFirst<TEntity>(SqlGetByKey, entity, transaction: transaction));
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                    return newModels;
                }
            }
        }


        public IEnumerable<TEntity> OffsetPaginate(int pageSize, int offset)
        {
            return OffsetPaginate(pageSize, offset, DefaultPageOrder);
        }
        public IEnumerable<TEntity> OffsetPaginate(int pageSize, int offset, string sqlPageOrder)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)} should be non-negative.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));

            string sql = string.Format("SELECT * FROM {0} ORDER BY {1} LIMIT {2} OFFSET {3};",
                TableName, sqlPageOrder, pageSize, offset);

            using (var cnn = GetNewConnection())
            {
                IEnumerable<TEntity> results = cnn.Query<TEntity>(sql);
                return results;
            }
        }

        public IEnumerable<TEntity> KeyedPaginate(int pageSize, TEntity lastKey)
        {
            if (lastKey == null) throw new ArgumentNullException(nameof(lastKey));
            return KeyedPaginate(pageSize, lastKey, DefaultPageOrder, DefaultKeyedPaginationKey);
        }
        public IEnumerable<TEntity> KeyedPaginate(int pageSize, TEntity lastKey, string sqlPageOrder, string sqlPaginationKey)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));
            if (sqlPaginationKey == null) throw new ArgumentNullException(nameof(sqlPaginationKey));
            if (lastKey == null) throw new ArgumentNullException(nameof(lastKey));

            string sql = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY {2} LIMIT {3};",
                TableName, sqlPaginationKey, sqlPageOrder, pageSize);

            using (var cnn = GetNewConnection())
            {
                IEnumerable<TEntity> results = cnn.Query<TEntity>(sql, lastKey);
                return results;
            }
        }

        //faster in most situations than get, but it was allways around 150ms,
        //which probably means that results may vary in real use cases
        public IEnumerable<TEntity> Search(Expression<Func<TEntity, bool>> filter)
        {
            var sql = $@"SELECT * FROM {TableName} {{where}}";

            using (var cnn = GetNewConnection())
            {
                IEnumerable<TEntity> results = cnn.Query<TEntity>(sql,  filter);
                return results;
            }
        }

        public long Count()
        {
            using (var cnn = GetNewConnection())
            {
                var output = cnn.QuerySingle<long>(SqlCount);
                return output;
            }
        }
        #endregion

        protected SQLiteConnection GetNewConnection()
        {
            var cnn = DbConnectionFactory.Invoke();
            if (cnn == null) 
                throw new NullReferenceException(
                        string.Format("Connection factory '{0}' produced null connection '{1}'.", 
                        nameof(DbConnectionFactory), nameof(cnn))
                    );
            return cnn;
        }
    }
}
