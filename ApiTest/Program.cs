using System;
using Dippy.DDApi;
using Dippy.DDApi.SQLite;
using Dippy.DDApi.SQLite.Repositories;
using Dippy.DDApi.DomainModels;


namespace ApiTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionBuilder = new SQLiteConnectionBuilder(@"H:\Documents\Projects\DD-API\DD Test.db");

            var repo = RepositoryFactory.GetSourceGroupRepository(connectionBuilder.BuildConnection);

            SourceGroup sourceGroup = repo.Get(1);

            DisplaySourceGroup(sourceGroup);

            //sourceGroup.Id = 1;
            //sourceGroup.Description = "A newer, bigger waste of time.";

            //repo.Update(sourceGroup);
            //sourceGroup = repo.Get(sourceGroup);

            //DisplaySourceGroup(sourceGroup);

            //repo.Delete(4); //no exception

            //Console.WriteLine("");
            //Console.WriteLine("Out Id:      {0}", 4);
            //Console.WriteLine("");

            ////sourceGroup = repo.Get(4); //throws on no results
            //DisplaySourceGroup(sourceGroup);
        }
        private static void DisplaySourceGroup(SourceGroup sourceGroup)
        {
            Console.WriteLine("Id:          {0}", sourceGroup.Id);
            Console.WriteLine("Name:        {0}", sourceGroup.Name ?? "null");
            Console.WriteLine("Url:         {0}", sourceGroup.Url ?? "null");
            Console.WriteLine("Description: {0}", sourceGroup.Description ?? "null");
            Console.WriteLine("TagId:       {0}", sourceGroup.TagId?.ToString());
        }
    }
}
