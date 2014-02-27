using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Configuration;

using Bfw.Common;
using Bfw.Common.Collections;
using Bfw.Common.Database;

using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;

namespace GenerateTaxonomy
{
    class Program
    {
        static ISessionManager SessionManager { get; set; }

        static DatabaseManager Database { get; set; }

        static void Main(string[] args)
        {
            Init();

            BuildTaxonomy(args[0]);

            Console.Write("\n\nPress any key to continue...");
            Console.ReadKey();
        }

        static void Init()
        {
            var user = ConfigurationManager.AppSettings["dlapUser"];
            var password = ConfigurationManager.AppSettings["dlapPassword"];

            SessionManager = new Bfw.Agilix.Dlap.Components.Session.ThreadSessionManager(null);
            SessionManager.CurrentSession = SessionManager.StartNewSession(user, password, string.Empty);

            Database = new DatabaseManager("PXTaxonomy");
        }

        static void BuildTaxonomy(string scope)
        {
            try
            {
                Database.StartSession();
                var sco = Database.Query("SaveTaxonomyScope @0", scope).Map(r => r.Int64("Id")).First();
                var search = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = scope,
                        Query = string.Format("/parent=\"{0}\"", "PX_TOC")
                    }
                };

                SessionManager.CurrentSession.Execute(search);

                foreach (var child in search.Items)
                {
                    BuildTaxonomy(scope, sco, 0, child);
                }
            }
            finally
            {
                Database.EndSession();
            }
        }

        static void BuildTaxonomy(string entity, long scope, long taxonomy, Item root)
        {
            if ((root.Type == DlapItemType.Folder || root.Type == DlapItemType.None) && !root.HiddenFromToc)
            {
                var tax = Database.Query("SaveTaxonomy @0, @1", root.Title, root.Id).Map(r => r.Int64("Id")).First();
                var search = new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = entity,
                        Query = string.Format("/parent=\"{0}\"", root.Id)
                    }
                };
                SessionManager.CurrentSession.Execute(search);

                foreach (var child in search.Items)
                {
                    BuildTaxonomy(entity, scope, tax, child);
                }
            }
            else if (!root.HiddenFromToc)
            {
                Database.ExecuteNonQuery("SaveTaxonomyItem @0, @1, @2, @3", scope, taxonomy, root.Id, root.Title);
            }
        }
    }
}
