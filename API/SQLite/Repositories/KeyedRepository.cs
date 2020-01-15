using System;
using System.Collections.Generic;
using System.Data.SQLite;
using Dapper;

namespace Dippy.DDApi.SQLite.Repositories
{
    public class KeyedRepository<TDomainModel> : IDomainModelRepository<TDomainModel> where TDomainModel : class
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
        protected string SqlCount { get; }
        
        protected Func<SQLiteConnection> DbConnectionFactory { get; }

        public KeyedRepository(Func<SQLiteConnection> dbConnectionFactory, string tableName, 
            string sqlInsertColumnNames, string sqlInsertValueNames, string sqlUpdate, string sqlKeyCondition,
            string defaultPageOrder, string defaultKeyedPaginationKey)
        {
            if (dbConnectionFactory == null)        throw new ArgumentNullException(nameof(dbConnectionFactory));
            if (tableName == null)                  throw new ArgumentNullException(nameof(tableName));
            if (sqlInsertColumnNames == null)       throw new ArgumentNullException(nameof(sqlInsertColumnNames));
            if (sqlInsertValueNames == null)        throw new ArgumentNullException(nameof(sqlInsertValueNames));
            if (sqlUpdate == null)                  throw new ArgumentNullException(nameof(sqlUpdate));
            if (sqlKeyCondition == null)            throw new ArgumentNullException(nameof(sqlKeyCondition));
            if (defaultPageOrder == null)           throw new ArgumentNullException(nameof(defaultPageOrder));
            if (defaultKeyedPaginationKey == null)  throw new ArgumentNullException(nameof(defaultKeyedPaginationKey));

            TableName = tableName;
            DefaultPageOrder = defaultPageOrder;
            DefaultKeyedPaginationKey = defaultKeyedPaginationKey;

            DbConnectionFactory = dbConnectionFactory;

            //you can sql inject yourself, if you wanted to
            

            //sqlInsertColumnNames = "Foo"; //order matters
            //sqlInsertValueNames = "@Foo"; //order matters
            //sqlInsertColumnNames = "Foo, Bar"; //order matters
            //sqlInsertValueNames = "@Foo, @Bar"; //order matters

            //sqlUpdate = "Foo = @Foo";
            //sqlUpdate = "Foo = @Foo, Bar = @Bar";

            //sqlKeyCondition = "Id1 = @Id1";
            //sqlKeyCondition = "Id1 = @Id1 AND Id2 = @Id2";

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


        public IEnumerable<TDomainModel> OffsetPaginate(int pageSize, int offset)
        {
            return OffsetPaginate(pageSize, offset, DefaultPageOrder);
        }
        public IEnumerable<TDomainModel> OffsetPaginate(int pageSize, int offset, string sqlPageOrder)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), $"{nameof(offset)} should be non-negative.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));

            string sql = string.Format("SELECT * FROM {0} ORDER BY {1} LIMIT {2} OFFSET {3};",
                TableName, sqlPageOrder, pageSize, offset);

            using (var cnn = GetNewConnection())
            {
                IEnumerable<TDomainModel> results = cnn.Query<TDomainModel>(sql);
                return results;
            }
        }

        public IEnumerable<TDomainModel> KeyedPaginate(int pageSize, TDomainModel lastKey)
        {
            if (lastKey == null) throw new ArgumentNullException(nameof(lastKey));
            return KeyedPaginate(pageSize, lastKey, DefaultPageOrder, DefaultKeyedPaginationKey);
        }
        public IEnumerable<TDomainModel> KeyedPaginate(int pageSize, TDomainModel lastKey, string sqlPageOrder, string sqlPaginationKey)
        {
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), $"{nameof(pageSize)} should be greater than zero.");
            if (sqlPageOrder == null) throw new ArgumentNullException(nameof(sqlPageOrder));
            if (sqlPaginationKey == null) throw new ArgumentNullException(nameof(sqlPaginationKey));
            if (lastKey == null) throw new ArgumentNullException(nameof(lastKey));

            string sql = string.Format("SELECT * FROM {0} WHERE {1} ORDER BY {2} LIMIT {3};",
                TableName, sqlPaginationKey, sqlPageOrder, pageSize);

            using (var cnn = GetNewConnection())
            {
                IEnumerable<TDomainModel> results = cnn.Query<TDomainModel>(sql, lastKey);
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
