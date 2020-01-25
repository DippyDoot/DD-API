using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
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

            var repo = (GenericRepository<SourceGroup>)RepositoryFactory.GetSourceGroupRepository(connectionBuilder.BuildConnection);

            var g = new SourceGroup() { Name = "Test Insert" };
            repo.Insert(g, out SourceGroup output);


            //IEnumerable<Tag> tags = repo.KeyedPaginate(3, new Tag() { Id = 2});
            //IEnumerable<Tag> tags = repo.KeyedPaginate(3,0);

            //foreach(Tag tag in tags)
            //{
            //    DisplayTag(tag);
            //    Console.WriteLine("");
            //}


            //var repo = RepositoryFactory.GetSourceGroupRepository(connectionBuilder.BuildConnection);

            //SourceGroup sourceGroup = repo.Get(1);

            DisplaySourceGroup(g);
            DisplaySourceGroup(output);

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
        private static void DisplayTag(Tag tag)
        {
            Console.WriteLine("Id:          {0}", tag.Id);
            Console.WriteLine("Name:        {0}", tag.Name ?? "null");
            Console.WriteLine("NCI:         {0}", tag.Name ?? "null");
            Console.WriteLine("TagTypeId:   {0}", tag.TagTypeId);
            Console.WriteLine("Description: {0}", tag.Description ?? "null");
            Console.WriteLine("Short Desc:  {0}", tag.ShortDescription ?? "null");
            Console.WriteLine("RFID:        {0}", tag.ResourceFileId?.ToString());
            Console.WriteLine("Rating:      {0}", tag.Rating?.ToString());

        }
    }
}
