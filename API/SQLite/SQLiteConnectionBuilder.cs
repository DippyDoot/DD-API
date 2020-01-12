using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Text;

namespace Dippy.DDApi.SQLite
{
    public class SQLiteConnectionBuilder
    {
        private SQLiteConnectionStringBuilder _connectionStringBuilder;

        private string _dbFilePath = string.Empty;
        public string DbFilePath 
        {
            get => _dbFilePath;
            set => _dbFilePath = value ?? string.Empty;
        }
        public bool ReadOnlyConnection { get; set; } = false;
        public bool FailIfDatabaseNotFound { get; set; } = true;
        public bool EnforceForeignKeysConstraint { get; set; } = true;

        public SQLiteConnectionBuilder(string dbFilePath = null)
        {
            _connectionStringBuilder = new SQLiteConnectionStringBuilder();
            DbFilePath = dbFilePath;
        }

        public string BuildConnectionString()
        {
            UpdateStringBuilder();
            return _connectionStringBuilder.ToString();
        }
        public SQLiteConnection BuildConnection()
        {
            return new SQLiteConnection(BuildConnectionString());
        }

        private void UpdateStringBuilder()
        {
            _connectionStringBuilder.DataSource = DbFilePath;
            _connectionStringBuilder.ReadOnly = ReadOnlyConnection;
            _connectionStringBuilder.FailIfMissing = FailIfDatabaseNotFound;
            _connectionStringBuilder.ForeignKeys = EnforceForeignKeysConstraint;
        }
    }
}
