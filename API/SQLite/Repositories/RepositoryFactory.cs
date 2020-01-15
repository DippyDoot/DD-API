using System;
using System.Data.SQLite;

using Dippy.DDApi.DomainModels;

namespace Dippy.DDApi.SQLite.Repositories
{
    public static class RepositoryFactory
    {
        public static IEntityRepository<TagType> GetTagTypeRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<TagType>(dbConnectionFactory, 
                "TagType", 
                "Name", 
                "@Name", 
                "Name = @Name");
        }

        public static IEntityRepository<TagGroup> GetTagGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<TagGroup>(dbConnectionFactory,
                "TagGroup", 
                "Name, TagTypeId", 
                "@Name, @TagTypeId", 
                "Name = @Name, TagTypeId = @TagTypeId");
        }

        public static IEntityRepository<SourceGroup> GetSourceGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<SourceGroup>(dbConnectionFactory,
                "SourceGroup",
                "Name, Url, Description, TagId", 
                "@Name, @Url, @Description, @TagId",
                "Name = @Name, Url = @Url, Description = @Description, TagId = @TagId");
        }

        public static IEntityRepository<Tag> GetTagRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<Tag>(dbConnectionFactory,
                "Tag",
                "Id, Name, NameCultureInfo, TagTypeId, Description, ShortDescription, ResourceFileId, Rating",
                "@Id, @Name, @NameCultureInfo, @TagTypeId, @Description, @ShortDescription, @ResourceFileId, @Rating",
                "Name = @Name, NameCultureInfo = @NameCultureInfo, TagTypeId = @TagTypeId, Description = @Description, ShortDescription = @ShortDescription, ResourceFileId = @ResourceFileId, Rating = @Rating");
        }
    }
}
