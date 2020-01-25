using System;
using System.Data.SQLite;

using Dippy.DDApi.DomainModels;

namespace Dippy.DDApi.SQLite.Repositories
{
    public static class RepositoryFactory
    {
        public static IRepository<TagType> GetTagTypeRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericRepository<TagType>(dbConnectionFactory, 
                RepoInitInfoBuilder.Get(typeof(TagType)));
        }

        public static IRepository<TagGroup> GetTagGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericRepository<TagGroup>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(TagGroup)));
        }

        public static IRepository<SourceGroup> GetSourceGroupRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericRepository<SourceGroup>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(SourceGroup)));
        }

        public static IRepository<Tag> GetTagRepository(Func<SQLiteConnection> dbConnectionFactory)
        {
            if (dbConnectionFactory == null) throw new ArgumentNullException(nameof(dbConnectionFactory));

            return new GenericRepository<Tag>(dbConnectionFactory,
                RepoInitInfoBuilder.Get(typeof(Tag)));
        }
    }
}
