using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Dippy.DDApi.SQLite.Repositories
{
    public class GenericEntityRepository<TEntity> : GenericKeyedRepository<TEntity>, IEntityRepository<TEntity> 
        where TEntity : class
    {
        protected string IdColumnName { get; }
        protected string SqlGetInsertedId { get; } //only works with auto increment PKs

        public GenericEntityRepository(Func<SQLiteConnection> dbConnectionFactory, string tableName, 
            string sqlInsertColumnNames, string sqlInsertValueNames, string sqlUpdate, 
            string sqlKeyCondition = "Id = @Id", string idColumnName = "Id") 
            : base(dbConnectionFactory, tableName, sqlInsertColumnNames, sqlInsertValueNames, sqlUpdate, sqlKeyCondition)
        {
            if (idColumnName == null) throw new ArgumentNullException(nameof(idColumnName));

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
                parameters.Add(IdColumnName, id);
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
                            parameters.Add(IdColumnName, id);
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
                parameters.Add(IdColumnName, id);
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
                            parameters.Add(IdColumnName, id);
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
        #endregion
    }
}
