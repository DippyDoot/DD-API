﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Dippy.DDApi.SQLite.Repositories
{
    public class EntityRepository<TEntity> : KeyedRepository<TEntity>, IEntityRepository<TEntity> 
        where TEntity : class
    {
        protected string IdParameterName { get; }
        protected string IdColumnName { get; }
        protected string SqlGetInsertedId { get; } //only works with auto increment PKs

        public EntityRepository(Func<SQLiteConnection> dbConnectionFactory, string tableName, 
            string sqlInsertColumnNames, string sqlInsertValueNames, string sqlUpdate, 
            string defaultPageOrder = "Id", string defaultKeyedPaginationKey = "Id > @Id",
            string sqlKeyCondition = "Id = @Id", string idParameterName = "Id", string idColumnName = "Id") 
            : base(dbConnectionFactory, tableName, sqlInsertColumnNames, sqlInsertValueNames, sqlUpdate, 
                  sqlKeyCondition, defaultPageOrder, defaultKeyedPaginationKey)
        {
            if (idParameterName == null)    throw new ArgumentNullException(nameof(idParameterName));
            if (idColumnName == null)       throw new ArgumentNullException(nameof(idColumnName));

            IdParameterName = idParameterName;
            IdColumnName = idColumnName;

            SqlGetInsertedId = string.Format("SELECT seq FROM sqlite_sequence WHERE name='{0}';", TableName);
        }

        #region IEntityRepository
        public void Insert(TEntity entity, out long id)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlInsertByKey, entity);
                id = cnn.QueryFirst<long>(SqlGetInsertedId);
            }
        }
        public void Insert(IEnumerable<TEntity> entities, out IEnumerable<long> ids)
        {
            List<long> newIds = new List<long>();

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
                            newIds.Add(cnn.QueryFirst<long>(SqlGetInsertedId));
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
            ids = newIds;
        }

        public void Delete(long id)
        {
            using (var cnn = GetNewConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add(IdParameterName, id);
                cnn.Execute(SqlDeleteByKey, parameters);
            }
        }
        public void Delete(IEnumerable<long> ids)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        foreach (long id in ids)
                        {
                            parameters.Add(IdParameterName, id);
                            cnn.Execute(SqlDeleteByKey, parameters, transaction: transaction);
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

        public TEntity Get(long id)
        {
            using (var cnn = GetNewConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add(IdParameterName, id);
                TEntity newEntity = cnn.QueryFirst<TEntity>(SqlGetByKey, parameters);
                return newEntity;
            }
        }
        public IEnumerable<TEntity> Get(IEnumerable<long> ids)
        {
            var newEntities = new List<TEntity>();

            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        var parameters = new DynamicParameters();
                        foreach (long id in ids)
                        {
                            parameters.Add(IdParameterName, id);
                            newEntities.Add(cnn.QueryFirst<TEntity>(SqlGetByKey, id, transaction: transaction));
                        }
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }

                    return newEntities;
                }
            }
        }

        public IEnumerable<long> OffsetPaginateId(int pageSize, int offset)
        {
            return OffsetPaginateId(pageSize, offset, DefaultPageOrder);
        }
        public IEnumerable<long> OffsetPaginateId(int pageSize, int offset, string sqlPageOrder)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)} should be non-negative.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));
            

            string sql = string.Format("SELECT {0} FROM {1} ORDER BY {2} LIMIT {3} OFFSET {4};",
                IdColumnName, TableName, sqlPageOrder, pageSize, offset);

            using (var cnn = GetNewConnection())
            {
                IEnumerable<long> results = cnn.Query<long>(sql);
                return results;
            }
        }


        public IEnumerable<TEntity> KeyedPaginate(int pageSize, long lastId)
        {
            return KeyedPaginate(pageSize, lastId, DefaultPageOrder, DefaultKeyedPaginationKey);
        }
        public IEnumerable<TEntity> KeyedPaginate(int pageSize, long lastId, string sqlPageOrder, string sqlPaginationKey)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));
            if (sqlPaginationKey == null) throw new ArgumentNullException(nameof(sqlPaginationKey));

            string sql = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY {2} LIMIT {3};",
                TableName, sqlPaginationKey, sqlPageOrder, pageSize);

            using (var cnn = GetNewConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add(IdParameterName, lastId);
                IEnumerable<TEntity> results = cnn.Query<TEntity>(sql, parameters);
                return results;
            }
        }

        public IEnumerable<long> KeyedPaginateId(int pageSize, long lastId)
        {
            return KeyedPaginateId(pageSize, lastId, DefaultPageOrder, DefaultKeyedPaginationKey);
        }
        public IEnumerable<long> KeyedPaginateId(int pageSize, long lastId, string sqlPageOrder, string sqlPaginationKey)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));

            string sql = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} LIMIT {4};",
                IdColumnName, TableName, sqlPaginationKey, sqlPageOrder, pageSize);

            using (var cnn = GetNewConnection())
            {
                var parameters = new DynamicParameters();
                parameters.Add(IdParameterName, lastId);
                IEnumerable<long> results = cnn.Query<long>(sql, parameters);
                return results;
            }
        }
        #endregion
    }
}
