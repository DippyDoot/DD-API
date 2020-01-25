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
                RepoInitInfoBuilder.Get(typeof(TagType)));
        }

        public static IEntityRepository<TagGroup> GetTagGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<TagGroup>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(TagGroup)));
        }

        public static IEntityRepository<SourceGroup> GetSourceGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<SourceGroup>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(SourceGroup)));
        }

        public static IEntityRepository<Tag> GetTagRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new EntityRepository<Tag>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(Tag)));
        }
    }
}
