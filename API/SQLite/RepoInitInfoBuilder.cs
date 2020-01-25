using Dippy.DDApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Dippy.DDApi.SQLite
{
    public static class RepoInitInfoBuilder
    {
        private class PropertyAttributeInfo
        {
            public string ColumnName { get; set; }
            public string ModelName { get; set; }
            public int PkOrder { get; set; }
            public bool IsReadOnly { get; set; }

            public PropertyAttributeInfo() { }

            public PropertyAttributeInfo(string columnName, string modelName, int pkOrder, bool isReadOnly)
            {
                ColumnName = columnName;
                ModelName = modelName;
                PkOrder = pkOrder;
                IsReadOnly = isReadOnly;
            }
        }

        private static Dictionary<Type, RepoInitInfo> _intializationInfoCache = new Dictionary<Type, RepoInitInfo>();

        /// <summary>
        /// Gets a copy of <see cref="RepoInitInfo"/> of <paramref name="type"/> from cache.
        /// <br>If it doesn't exist, it creates one, caches it, and returns a copy of the cache.</br>
        /// </summary>
        /// <param name="type">Type that the info is for.</param>
        /// <returns>Returns a copy of <see cref="RepoInitInfo"/> from cache.</returns>
        public static RepoInitInfo Get(Type type)
        {
            if (_intializationInfoCache.TryGetValue(type, out RepoInitInfo info))
                return (RepoInitInfo)info.Clone();

            info = CreateInitializationInfo(type);

            if (info != null)
                _intializationInfoCache.Add(type, info);
            else
                throw new Exception($"{nameof(info)} is null, but that should have thrown an error before this was reached.");

            return info;
        }

        private static RepoInitInfo CreateInitializationInfo(Type type)
        {
            var info = new RepoInitInfo();

            List<PropertyAttributeInfo> columnInfos = GetPropertyAttributeInfos(GetProperties(type));
            ThrowIfNoKey(columnInfos, type.Name);

            //get pks from columnInfos and order them
            List<(string columnName, string modelName)> primaryKeys =
                columnInfos.Where(x => x.PkOrder >= 0)
                           .OrderBy(x => x.PkOrder)
                           .Select(x => (columnName: x.ColumnName, modelName: x.ModelName))
                           .ToList();
            //get writable columns from columnInfos
            List<(string columnName, string modelName)> writeableColumns =
                columnInfos.Where(x => !x.IsReadOnly)
                           .Select(x => (columnName: x.ColumnName, modelName: x.ModelName))
                           .ToList();

            //cache for readability
            string keyCondition = null;
            string update = null;
            string columnNames = null; //order matters
            string modelNames = null; //order matters
            string orderByKey = null;
            
            //build out value statements
            keyCondition = StatementValueBuilder(" AND ", "{0} = @{1}", 9, primaryKeys);
            update = StatementValueBuilder(", ", "{0} = @{1}", 6, writeableColumns);
            columnNames = StatementValueBuilder(", ", "{0}", 2, writeableColumns);
            modelNames = StatementValueBuilder(", ", "@{0}", 3, writeableColumns);
            orderByKey = StatementValueBuilder(", ", "{0}", 2, primaryKeys);

            //instert value statements into sql querries
            info.TableName = GetTableName(type);
            info.SqlInsertByKey = string.Format("INSERT INTO {0} ({1}) VALUES ({2})", info.TableName, columnNames, modelNames);
            info.SqlUpdateByKey = string.Format("UPDATE {0} SET {1} WHERE {2}", info.TableName, update, keyCondition);
            info.SqlDeleteByKey = string.Format("DELETE FROM {0} WHERE {1}", info.TableName, keyCondition);
            info.SqlGetByKey = string.Format("SELECT * FROM {0} WHERE {1}", info.TableName, keyCondition);
            info.SqlGetLastInserted = string.Format("SELECT * FROM {0} WHERE _rowid_ = (SELECT last_insert_rowid())", info.TableName);
            info.SqlCount = string.Format("SELECT COUNT(*) FROM {0}", info.TableName);
            info.OrderByKey = orderByKey;
            info.DefaultKeyedPaginationKey = string.Format("({0}) > ({1})", columnNames, modelNames);

            return info;
        }

        /// <summary>
        /// Gets the table name of <paramref name="type"/>.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Returns the <see cref="TableNameAttribute.Name"/> if it exists or the name of the type.</returns>
        private static string GetTableName(Type type)
        {
            string tableName = null;

            Attribute[] attributes = Attribute.GetCustomAttributes(type);

            if (attributes.Length > 0)
            {
                foreach (Attribute attribute in attributes)
                {
                    if (attribute is TableNameAttribute tn)
                    {
                        tableName = tn.Name;
                        break;
                    }
                }
            }

            if (tableName == null)
                tableName = type.Name;

            return tableName;
        }

        /// <summary>
        /// Gets public properties of <paramref name="type"/> with a public get and a public set.
        /// </summary>
        /// <param name="type">Type that the properties are gotten from.</param>
        /// <returns>A list of PropertyInfo containing atleast one element.</returns>
        /// <exception cref="Exception">
        /// Thrown when <paramref name="type"/> has no public properties with a public get and a public set.
        /// </exception>
        private static List<PropertyInfo> GetProperties(Type type)
        {
            List<PropertyInfo> properties = new List<PropertyInfo>();

            PropertyInfo[] tmp = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (tmp.Length == 0) //getprops returns empty if none
                throw new Exception($"{type.Name} has no public properties.");

            foreach (PropertyInfo p in tmp)
            {
                //if no get set, continue
                if (!p.CanWrite || !p.CanRead) continue;

                //Get get set of prop. If not public get or set exists, continue
                if (p.GetGetMethod(false) == null) continue;
                if (p.GetSetMethod(false) == null) continue;

                //now that we know it is a valid prop, add to list
                properties.Add(p);
            }

            if (properties.Count == 0)
                throw new Exception($"{type.Name} has no properties with a public get and a public set.");

            return properties;
        }

        /// <summary>
        /// Gets attribute information of <paramref name="properties"/>.
        /// </summary>
        /// <param name="properties">The properties that the attribute information is gotten from.</param>
        /// <returns>A list of <see cref="PropertyAttributeInfo"/>. If no relavent info is fround, then the list is empty.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is null.</exception>
        private static List<PropertyAttributeInfo> GetPropertyAttributeInfos(IEnumerable<PropertyInfo> properties)
        {
            if (properties == null) throw new ArgumentNullException(nameof(properties));

            var columnInfos = new List<PropertyAttributeInfo>();

            //fill out columnInfos 
            foreach (PropertyInfo p in properties)
            {
                IEnumerable<Attribute> attributes = p.GetCustomAttributes();
                var columnInfo = new PropertyAttributeInfo(null, null, -1, false);

                //get atribute data
                foreach (Attribute attr in attributes)
                {
                    switch (attr)
                    {
                        case ColumnNameAttribute cn:
                            columnInfo.ColumnName = cn.Name;
                            break;
                        case ReadOnlyColumnAttribute _:
                            columnInfo.IsReadOnly = true;
                            break;
                        case KeyAttribute key:
                            columnInfo.PkOrder = key.Order;
                            //if true, assign true, else do nothing (doesn't override if already true)
                            columnInfo.IsReadOnly = key.IsAutoGenerated | columnInfo.IsReadOnly;
                            break;
                    }
                }

                columnInfo.ModelName = p.Name;
                if (columnInfo.ColumnName == null)
                    columnInfo.ColumnName = p.Name;

                columnInfos.Add(columnInfo);
            }

            return columnInfos;
        }

        /// <summary>
        /// Builds out the value part of the SQL statment, eg Update "Foo = @Foo" ...
        /// </summary>
        /// <param name="seperator">Seperator that is used in each repetition of the pattern.</param>
        /// <param name="skeleton">The part that uses <see cref="String.Format(string, object[])"/>.</param>
        /// <param name="staticSize">
        /// The total size of static parts of both <paramref name="seperator"/> and <paramref name="skeleton"/> that don't get replaced. 
        /// <br>Used for builder size allocation.</br>
        /// </param>
        /// <param name="values">The values used for replacement.</param>
        /// <returns>
        /// Returns a string that folows the pattern of <paramref name="skeleton"/> 
        /// replaced by <paramref name="values"/> with <paramref name="seperator"/> inbtween.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown when any prarmeter is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="values"/> contains 0 elements.</exception>
        /// <remarks>
        /// <paramref name="skeleton"/> should only contain "{0}" and/or "{1}" indexed replacements, else an error will be thrown.
        /// </remarks>
        private static string StatementValueBuilder(string seperator, string skeleton, int staticSize, List<(string columnName, string modelName)> values)
        {
            if (skeleton == null)   throw new ArgumentNullException(nameof(skeleton));
            if (seperator == null)  throw new ArgumentNullException(nameof(seperator));
            if (values == null)     throw new ArgumentNullException(nameof(values));
            if (values.Count == 0)  throw new ArgumentException($"{nameof(values)} should not be empty.");

            var builder = new StringBuilder(CalcUpperBound(staticSize, values.Count));

            //0 loop
            builder.Append(string.Format(skeleton, values[0].columnName, values[0].modelName));
            //i>=1 loop
            for (int i = 1; i < values.Count; i++)
            {
                builder.Append(seperator);
                builder.Append(string.Format(skeleton, values[i].columnName, values[i].modelName));
            }

            return builder.ToString();
        }

        /// <summary>
        /// Calculates a resonable upper bound for a StringBuilder.
        /// </summary>
        /// <param name="staticSize">Size of the non-word part of the pattern.</param>
        /// <param name="count">Total times the pattern is repeated.</param>
        /// <param name="allottedWordSize">
        /// Size to allocate for the word. This value is doubled as it is assumed that it will be used twice.</param>
        /// <returns>Returns an int.</returns>
        private static int CalcUpperBound(int staticSize, int count, int allottedWordSize = 14)
        {
            return (staticSize + allottedWordSize * 2) * count;
        }

        /// <summary>
        /// Throws Exception if no property is marked with a <see cref="KeyAttribute"/> attribute.
        /// </summary>
        /// <param name="columnInfos">The properties that are checked.</param>
        /// <param name="modelName">The name that is used when the exception is thrown.</param>
        /// <exception cref="Exception">
        /// Thrown when <paramref name="columnInfos"/> has no properties marked with a <see cref="KeyAttribute"/> attribute.
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnInfos"/> is null.</exception>
        private static void ThrowIfNoKey(List<PropertyAttributeInfo> columnInfos, string modelName)
        {
            if (columnInfos == null) throw new ArgumentNullException(nameof(columnInfos));

            bool hasKey = false;
            for (int i = 0; i < columnInfos.Count; i++)
            {
                if (columnInfos[i].PkOrder >= 0)
                {
                    hasKey = true;
                    break;
                }
            }
            if (!hasKey)
                throw new Exception(
                    string.Format("{0} does not have any properties marked with a {1} attribute.",
                    modelName, nameof(KeyAttribute))
                    );
        }

    }
}
