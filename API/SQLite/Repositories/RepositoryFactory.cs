using System;
using System.Data.SQLite;

using Dippy.DDApi.DomainModels;

namespace Dippy.DDApi.SQLite.Repositories
{
    public static class RepositoryFactory
    {
        public static GenericEntityRepository<TagType> GetTagTypeRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericEntityRepository<TagType>(dbConnectionFactory, 
                "TagType", 
                "Name", "@Name", 
                "Name = @Name");
        }

        public static GenericEntityRepository<TagGroup> GetTagGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericEntityRepository<TagGroup>(dbConnectionFactory,
                "TagGroup", 
                "Name, TagTypeId", "@Name, @TagTypeId", 
                "Name = @Name, TagTypeId = @TagTypeId");
        }

        public static GenericEntityRepository<SourceGroup> GetSourceGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericEntityRepository<SourceGroup>(dbConnectionFactory,
                "SourceGroup",
                "Name, Url, Description, TagId", "@Name, @Url, @Description, @TagId",
                "Name = @Name, Url = @Url, Description = @Description, TagId = @TagId");
        }
    }
}
