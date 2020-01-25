using System;
using System.Collections.Generic;
using System.Text;

namespace Dippy.DDApi.SQLite
{
    public class RepoInitInfo : ICloneable
    {
        public string TableName { get; set; }
        public string SqlInsertByKey { get; set; }
        public string SqlUpdateByKey { get; set; }
        public string SqlDeleteByKey { get; set; }
        public string SqlGetByKey { get; set; }
        public string SqlGetLastInserted { get; set; }
        public string SqlCount { get; set; }
        public string OrderByKey { get; set; }
        public string DefaultKeyedPaginationKey { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
