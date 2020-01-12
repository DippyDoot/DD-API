using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Dippy.DDApi.SQLite.Repositories
{
    public abstract class GenericKeyedRepository<TDomainModel> : IDomainModelRepository<TDomainModel> where TDomainModel : class
    {
        protected string TableName { get; }
        protected string SqlInsertByKey { get; }
        protected string SqlUpdateByKey { get; }
        protected string SqlDeleteByKey { get; }
        protected string SqlGetByKey { get; }
        protected string SqlCount { get; }
        
        protected Func<SQLiteConnection> DbConnectionFactory { get; }

        public GenericKeyedRepository(Func<SQLiteConnection> dbConnectionFactory, string tableName, 
            string sqlInsertColumnNames, string sqlInsertValueNames, string sqlUpdate, string sqlKeyCondition)
        {
            if (dbConnectionFactory == null) throw new NotImplementedException(nameof(dbConnectionFactory));
            if (tableName == null) throw new NotImplementedException(nameof(tableName));
            if (sqlInsertColumnNames == null) throw new NotImplementedException(nameof(sqlInsertColumnNames));
            if (sqlInsertValueNames == null) throw new NotImplementedException(nameof(sqlInsertValueNames));
            if (sqlUpdate == null) throw new NotImplementedException(nameof(sqlUpdate));
            if (sqlKeyCondition == null) throw new NotImplementedException(nameof(sqlKeyCondition));

            TableName = tableName;
            DbConnectionFactory = dbConnectionFactory;

            //you can sql inject yourself, if you wanted to
            

            //string sqlInsertColumnNames = "Name"; //order matters, comma seperated
            //string sqlInsertValueNames = "@Name"; //order matters, comma seperated
            //string sqlUpdate = "Name = @Name"; //comma seperated
            //string sqlKeyCondition = "Id = @Id";

            SqlInsertByKey = string.Format("INSERT INTO {0} ({1}) VALUES ({2});", TableName, sqlInsertColumnNames, sqlInsertValueNames);
            SqlUpdateByKey = string.Format("UPDATE {0} SET {1} WHERE {2};", TableName, sqlUpdate, sqlKeyCondition);
            SqlDeleteByKey = string.Format("DELETE FROM {0} WHERE {1};", TableName, sqlKeyCondition);
            SqlGetByKey = string.Format("SELECT * FROM {0} WHERE {1};", TableName, sqlKeyCondition);
            SqlCount = string.Format("SELECT COUNT(*) FROM {0};", TableName);
        }


        #region IDomainModelRepository
        public void Insert(TDomainModel model)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlInsertByKey, model);
            }
        }
        public void Insert(IEnumerable<TDomainModel> models) 
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TDomainModel model in models)
                        {
                            cnn.Execute(SqlInsertByKey, model, transaction: transaction);
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

        public void Update(TDomainModel model)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlUpdateByKey, model);
            }
        }
        public void Update(IEnumerable<TDomainModel> models)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TDomainModel model in models)
                        {
                            cnn.Execute(SqlUpdateByKey, model, transaction: transaction);
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

        public void Delete(TDomainModel model)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Execute(SqlDeleteByKey, model);
            }
        }
        public void Delete(IEnumerable<TDomainModel> models)
        {
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TDomainModel model in models)
                        {
                            cnn.Execute(SqlDeleteByKey, model, transaction: transaction);
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

        public TDomainModel Get(TDomainModel model)
        {
            using (var cnn = GetNewConnection())
            {
                TDomainModel newModel = cnn.QueryFirst<TDomainModel>(SqlGetByKey, model);
                return newModel;
            }
        }
        public IEnumerable<TDomainModel> Get(IEnumerable<TDomainModel> models)
        {
            var newModels = new List<TDomainModel>();
            using (var cnn = GetNewConnection())
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    try
                    {
                        foreach (TDomainModel model in models)
                        {
                            newModels.Add(cnn.QueryFirst<TDomainModel>(SqlGetByKey, model, transaction: transaction));
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
