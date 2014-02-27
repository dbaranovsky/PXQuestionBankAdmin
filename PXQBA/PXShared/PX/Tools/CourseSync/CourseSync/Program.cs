using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Configuration;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Common.Collections;
using Bfw.Common.Patterns.Unity;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.Dlap.Components.Session;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.PX.Biz.DataContracts;
using Bfw.PX.Biz.Services.Mappers;
using Bfw.PX.Biz.Direct.Services;
using BizSC = Bfw.PX.Biz.ServiceContracts;
using Mono.Options;
using System.IO;

namespace CourseSync
{
    class Program
    {
        private static ISession Session { get; set; }
        private static BizSC.IContentActions ContentActions { get; set; }
        private static BizSC.IBusinessContext BusinessContext { get; set; }
        static void Main(string[] args)
        {
            
            ConfigureServiceLocator();

            var tracer = ServiceLocator.Current.GetInstance<Bfw.Common.Logging.ITraceManager>();
            if (tracer != null)
                tracer.StartTracing();

            var options = new CommandOptions();
            var os = RegisterOptions(options);
            var sm = EstablishConnection();
            Session = sm.CurrentSession;
            var toProcess = new Dictionary<string, ContentItem>();
            var processed = new List<Item>();


            os.Parse(args);

            /* invocation entails passing params to cli as follows:
             *      /c:9 /f:PX_FLASHCARDS /v:ebook /a:search
             *      
             *      where: 
             *      /c = courseid
             *      /f = folderid
             *      /v = the catagories you wish to assign comma separated
             *      
             * Currently, category options are:
             *  - ebook
             *  - other 
             *  - exclude
             * 
             *      /a = the action to perform. By specifying "search" the command will apply
             *          the search categories. Leaving this out will result in the program processing
             *          metadata as it was originally intended to do.
             */
            string action = string.Empty;
            string line = string.Empty;


            do {
                //58320
                Console.WriteLine("Enter in the commands, and properties you want to run against Dlap.");
                line = Console.ReadLine();
                options = new CommandOptions();
                os = RegisterOptions(options);
                string[] g = line.Split(' ');
                os.Parse(g);

                action = string.IsNullOrEmpty(options.Action) ? "" : options.Action.ToLowerInvariant();

                switch (action)
                {
                    case "updatecourses":
                        Console.WriteLine("Updating courses...");
                        UpdateCourses(options.DestinationEnvironment, options.FileName, "");
                        break;

                    case "copyitem":
                        Console.WriteLine("Copying Item...");
                        CopyItem(options.DestinationItemId, options.SourceItemId, options.DestinationEnvironment, options.SourceEnvironment, options.DestinationEntityId, options.SourceEntityId);
                        break;

                    case "copyitems":
                        Console.WriteLine("Copying Items...");
                        CopyItems(options.SourceEntityId, options.DestinationEntityId, options.SourceEnvironment, options.DestinationEnvironment);
                        break;
                         
                    case "copyresources":
                        Console.WriteLine("Copying Resource...");
                        CopyResources(options.DestinationEntityId, options.SourceEntityId, options.SourcePath, options.DestinationPath, options.SourceEnvironment, options.DestinationEnvironment);
                        break;

                    case "copypagedefinitions":
                        Console.WriteLine("Copying Page Definitions...");
                        CopyPageDefinitions(options.SourceEntityId, options.SourceItemId, options.DestinationEntityId, options.SourceEnvironment, options.DestinationEnvironment);
                    break;    

                    case "deleteitems":
                    Console.WriteLine("Deleting Items...");
                    DeleteItems(options.SourceEntityId, options.SourceItemId, options.SourceEnvironment, Convert.ToBoolean(options.Recursive));
                    break;

                    default:
                        Console.WriteLine(string.Format("Could not find the command you requested:\n Action={0}\n To Entity Id={1}\n From Environment={2}\n From Item Id={3}\n To Entity Id={4}\n To Environment={5}\n To Item Id={6}", options.Action, options.SourceEntityId, options.DestinationEnvironment, options.DestinationItemId, options.SourceEntityId, options.SourceEnvironment, options.SourceItemId));
                        break;


                }
            }
            while (line != "quit");
                
           
                //SyncItems(options.CourseId, options.ProductCourseId, "Dev", "QA");
                //SyncItems("9631", "6747", "Dev", "Dev"); //Krugman
                //SyncItems("9631", "6682", "Dev", "Dev"); //Freedman
                //SyncItems("9631", "6646", "Dev", "Dev"); //Myers
                //SyncItems("9631", "6650", "Dev", "Dev"); //Phelan
                //SyncItems("9631", "9228", "Dev", "Dev"); //Bps5e/Moore
                //SyncItems("9631", "6440", "Dev", "Dev"); //Hockenbury

                //SyncItems("9631", "11952", "Dev", "QA"); //Hockenbury deriv to Hockenbury QA
                //SyncItems("9631", "12079", "Dev", "QA"); //Hockenbury deriv to Freeman QA
                //SyncItems("9631", "12082", "Dev", "QA"); //Hockenbury deriv to Krugman QA
                //SyncItems("9631", "12101", "Dev", "QA"); //Hockenbury deriv to Myers QA
                //SyncItems("9631", "12107", "Dev", "QA"); //Hockenbury deriv to Phelan QA
                //SyncItems("9631", "12110", "Dev", "QA"); //Hockenbury deriv to Bps5e QA

                //SyncItems("9631", "12113", "Dev", "QA"); //Hockenbury deriv to ePortfolio QA            
                //SyncItems("9631", "12124", "Dev", "QA"); //Hockenbury deriv to xBook QA
                //SyncItems("9631", "12130", "Dev", "QA"); //Hockenbury deriv to CompClass QA



                //SyncItems("11952", "11169", "QA", "Prod");//ePortfolio deriv to Hockenbury QA            
                //SyncItems("12079", "11935", "QA", "Prod");//ePortfolio deriv to Freeman QA            
                //SyncItems("12082", "11969", "QA", "Prod");//ePortfolio deriv to Krugman QA            
                //SyncItems("12110", "11159", "QA", "Prod");//ePortfolio deriv to Bps5e QA

                // #############
                //SyncItems("12101", "11533", "QA", "Prod");//ePortfolio deriv to Myers QA   
                //SyncItems("12107", "11535", "QA", "Prod");//ePortfolio deriv to Phelan QA 
                // #############


                //SyncItems("12113", "9102", "QA", "Prod");//ePortfolio deriv to ePortfolio QA            
                //SyncItems("12124", "11134", "QA", "Prod"); //Hockenbury deriv to xBook QA
                //SyncItems("12130", "11132", "QA", "Prod"); //Hockenbury deriv to CompClass QA


                //SyncItems("1", "12", "Prod", "QA"); //Hockenbury to Hockenbury DEV-QA        
                //SyncItems("12130", "12124", "QA", "QA"); //Hockenbury deriv to CompClass QA


            

        }

        /// <summary>
        /// Accepts list of courseids to update (csv) and updates with free xml
        /// </summary>
        /// <param name="environment"></param>
        /// <param name="fileName"></param>
        /// <param name="xml"></param>
        private static void UpdateCourses(string environment, string fileName, string xml)
        {
            // usage
            // environment = "qa";
            // fileName = "qa.csv";
            // xml = "<data><bfw_allow_sampling>true</bfw_allow_sampling><bfw_allow_purchase>true</bfw_allow_purchase></data>";

            // check for filename
            if (fileName.IsNullOrEmpty())
            {
                Console.WriteLine("The file name with list of courses was not specified!");
                return;
            }

            // check for xml 
            if (xml.IsNullOrEmpty())
            {
                Console.WriteLine("The xml was not specified!");
                return;
            }

            XElement doc = null;

            // parse xml
            try
            {
                doc = XElement.Parse(xml);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            // create a list of courses to update
            List<string> courses = new List<string>();

            using (FileStream file = new FileStream(fileName, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(file))
                {
                    var course = string.Empty;

                    while (!reader.EndOfStream)
                    {
                        course = reader.ReadLine();
                        courses.Add(course);
                    }

                    reader.Close();
                }

                file.Close();
            }

            // update courses
            foreach (var course in courses)
            {
                Console.WriteLine("Reading course #{0} ...", course);

                GetCourse getCourse = new GetCourse()
                {
                    SearchParameters = new CourseSearch() { CourseId = course }
                };

                try
                {
                    Session.ExecuteAsAdmin(environment, getCourse);

                    Console.WriteLine("Updating course #{0} ...", course);

                    if (getCourse.Courses.Count() > 0)
                    {
                        var Course = getCourse.Courses.First().ToCourse().ToCourse().ToEntity();

                        foreach (var node in doc.Elements())
                        {
                            var elem = Course.Descendants(doc.Name).Elements().FirstOrDefault(o => o.Name == node.Name);

                            if (elem == null)
                            {
                                var q = (from s in Course.Elements(doc.Name) select s).FirstOrDefault();

                                q.Add(node);
                            }
                            else
                            {
                                elem.Value = node.Value;
                            }
                        }

                        var request = new DlapRequest()
                        {
                            Type = DlapRequestType.Post,
                            Mode = DlapRequestMode.Batch,
                            Parameters = new Dictionary<string, object>() { { "cmd", "updatecourses" } }
                        };

                        request.AppendData(Course);

                        Session.Send(request, true, environment);
                    }
                }
                catch (Exception ex)
                {
                    Console.Write(", failed: {0}", ex.Message);
                    continue;
                }
            }
        }

        private static void CopyItem(string fromItemId, string toItemId, string fromEnvironment, string toEnvironment, string fromEntityId, string toEntityId)
        {
            if (fromEntityId.IsNullOrEmpty() || fromItemId.IsNullOrEmpty() || toItemId.IsNullOrEmpty() || fromEnvironment.IsNullOrEmpty() || toEnvironment.IsNullOrEmpty() || fromEntityId.IsNullOrEmpty() || toEntityId.IsNullOrEmpty())
            {
                Console.WriteLine("You left one of the required parameters blank."); 
                return;
            }
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = fromEntityId,
                    ItemId = fromItemId
                }
            };
            Session.ExecuteAsAdmin(fromEnvironment,query);
            if (query.Items.Count <= 0)
            {
                Console.WriteLine("Couldn't find the item you requested.");
                return;

            }
            else
            {
                var putquery = new PutItems();
                Item item = query.Items[0];
                item.EntityId = toEntityId;
                putquery.Add(item);
                Session.ExecuteAsAdmin(toEnvironment, putquery);

                Console.WriteLine("Copy Complete...");
            }
        }

        /// <summary>
        /// Loads all UI items under childCourseId and puts them into the productCourseId
        /// </summary>
        /// <param name="childCourseId">course to grab items from</param>
        /// <param name="productCourseId">course to put items to</param>
        private static void SyncItems(string childCourseId, string productCourseId, string from, string to)
        {
            Boolean moveMultiPartLessons = false;
            Boolean movePxTemplates = false;
            Boolean moveResources = true;
            Boolean moveWidgets = true;
            Boolean moveNavigation = true;

            var parentIds = "";

            if (moveWidgets) parentIds += ",PX_LOCATION_ZONE2,PX_LOCATION_ZONE3,PX_LOCATION_ZONE1_MENU_1";
            if (moveNavigation) parentIds += ",PX_LOCATION_ZONE1_MENU0";
            if (moveMultiPartLessons) parentIds += ",PX_MULTIPART_LESSONS";
            if (movePxTemplates) parentIds += ",PX_TEMPLATES";


            var deleteIds = "";

            if (moveWidgets) deleteIds += ",PX_LOCATION_ZONE2,PX_LOCATION_ZONE3,PX_LOCATION_ZONE1_MENU_1";
            if (moveNavigation) deleteIds += ",PX_LOCATION_ZONE1_MENU0";
            if (moveMultiPartLessons) deleteIds += ",PX_MULTIPART_LESSONS";
            if (movePxTemplates) deleteIds += ",PX_TEMPLATES";

            List<String> parentIdsToCopy = parentIds.Split(',').ToList();
            List<String> parentIdsToDelete = deleteIds.Split(',').ToList();

            List<Item> itemsToCopy = new List<Item>();
            List<String> resourcesToCopy = new List<String>();

            foreach (String parentId in parentIdsToDelete)
            {
                var cmd = ListChildren(productCourseId, parentId, 2);
                Session.ExecuteAsAdmin(to, cmd);

                if (!cmd.Items.IsNullOrEmpty())
                {
                    var cmdDelete = new DeleteItems()
                    {
                        Items = cmd.Items
                    };
                    Session.ExecuteAsAdmin(to, cmdDelete);
                }
            }

            foreach (String parentId in parentIdsToCopy)
            {
                var cmd = ListChildren(childCourseId, parentId, 2);
                Session.ExecuteAsAdmin(from, cmd);
                if (!cmd.Items.IsNullOrEmpty())
                {
                    itemsToCopy.AddRange(cmd.Items);
                }
            }

            //Copy multipartlessons
            if (moveMultiPartLessons)
            {

                var cmdLessons = ListChildren(childCourseId, "PX_MULTIPART_LESSONS", 2);
                Session.ExecuteAsAdmin(from, cmdLessons);
                if (!cmdLessons.Items.IsNullOrEmpty())
                {
                    itemsToCopy.AddRange(cmdLessons.Items);

                    foreach (var lesson in cmdLessons.Items)
                    {
                        var cmdLessonItems = ListChildren(childCourseId, lesson.Id);
                        Session.ExecuteAsAdmin(from, cmdLessonItems);

                        if (!cmdLessonItems.Items.IsNullOrEmpty())
                        {
                            itemsToCopy.AddRange(cmdLessonItems.Items);
                        }

                        cmdLessonItems = ListChildren(childCourseId, lesson.Id, 1, lesson.Id);
                        Session.ExecuteAsAdmin(from, cmdLessonItems);

                        if (!cmdLessonItems.Items.IsNullOrEmpty())
                        {
                            itemsToCopy.AddRange(cmdLessonItems.Items);
                        }
                    }
                }
            }

            if (movePxTemplates)
            {
                //Copy Template Descriptions
                var cmdTemplates = ListChildren(childCourseId, "PX_TEMPLATES", 2);
                Session.ExecuteAsAdmin(from, cmdTemplates);
                if (!cmdTemplates.Items.IsNullOrEmpty())
                {
                    //itemsToCopy.AddRange(cmdLessons.Items);

                    foreach (var template in cmdTemplates.Items)
                    {
                        var cmdTemplateItems = ListChildren(childCourseId, template.Id);
                        Session.ExecuteAsAdmin(from, cmdTemplateItems);

                        if (!cmdTemplateItems.Items.IsNullOrEmpty())
                        {
                            itemsToCopy.AddRange(cmdTemplateItems.Items);
                        }

                    }

                    CopyResources(childCourseId, productCourseId, "Templates/Data/TMP_*", "Templates/Data/", from, to);
                }
            }

            foreach (var item in itemsToCopy)
            {
                item.EntityId = productCourseId;
                if (item.Title == "CONTENT") item.Title = "Content";
            }

            StoreItems(productCourseId, itemsToCopy, to);

            if (moveResources)
            {
                CopyResources(childCourseId, productCourseId, "Templates/Data/*", "Templates/Data/", from, to);
            }



        }

        private static void CopyPageDefinitions(string sourceEntityId, string sourceItemId, string destinationEntityId, string sourceEnvironment, string destinationEnvironment)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = sourceEntityId,
                    ItemId = sourceItemId
                }
            };
            Session.ExecuteAsAdmin(sourceEnvironment, query);
            if (query.Items == null)
            {
                Console.WriteLine("Could not find the root xml you requested");
                return;
            }
            List<Item> items = new List<Item>();
            List<Item> childrenItems = new List<Item>();
            ItemQueryHelper.ListDescendentingItems(Session, sourceEntityId, ContentActions, query.Items[0], items, childrenItems, sourceEnvironment);

            if (items == null)
            {
                Console.WriteLine("Could not find the page definitions");
                return;
            }

            foreach (Item i in items)
            {
                i.EntityId = destinationEntityId;
            }

            var putquery = new PutItems();
            putquery.Add(items);
            Session.ExecuteAsAdmin(destinationEnvironment, putquery);
            Console.WriteLine("Copy Page Definitions Complete...");
        }
        private static void DeleteItems(string sourceEntityId, string sourceItemId, string sourceEnvironment, bool recursive = false)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = sourceEntityId,
                    ItemId = sourceItemId
                }
            };
            Session.ExecuteAsAdmin(sourceEnvironment, query);
            if (query.Items == null)
            {
                Console.WriteLine("Could not find the root xml you requested");
                return;
            }
            List<Item> items = new List<Item>();
            if (recursive == true)
            {

                List<Item> childrenItems = new List<Item>();
                ItemQueryHelper.ListDescendentingItems(Session, sourceEntityId, ContentActions, query.Items[0], items, childrenItems, sourceEnvironment);

                if (items == null || items.Count() <= 0)
                {
                    Console.WriteLine("Could not find children elements");
                    return;
                }
            }
            else
            {
                items.Add(query.Items[0]);
            }
            foreach (Item i in items)
            {
                i.ParentId = "PX_DELETED";
            }

            var putquery = new PutItems();
            putquery.Add(items);
            Session.ExecuteAsAdmin(sourceEnvironment, putquery);
            Console.WriteLine("Deleting Items Completed...");
        }

        private static void CopyItems(string sourceEntityId, string destinationEntityId, string sourceEnvironment, string destinationEnvironment)
        {
            Console.WriteLine("Enter the item ids for the items you want to copy. When you are finished enter the word 'done'. ");
           string input = Console.ReadLine();
           
           List<string> itemIds = new List<string>();
            while(input != "done"){
                itemIds.Add(input);
                input = Console.ReadLine();
            }
            var batch = new Batch();

            foreach (string i in itemIds)
            {
                batch.Add(new GetItems()
                {
                    SearchParameters = new ItemSearch()
                    {
                        EntityId = sourceEntityId,
                        ItemId = i
                    }
                });
            }
            Session.ExecuteAsAdmin(sourceEnvironment, batch);
            
            
            List<Item> items = new List<Item>();
            List<string> itemsNotFound = new List<string>();
            List<DlapCommand> commands = new List<DlapCommand>();


           var getItemsList = batch.Commands.Map(i => i as GetItems).ToList();
           int whereInTheListAmI = 0;

           foreach (List<Item> i in getItemsList.Map(i => i.Items))
           {

               if (i.Count <= 0)
               {
                   itemsNotFound.Add(getItemsList[whereInTheListAmI].SearchParameters.ItemId);
               }
               else
               {
                   items.Add(i[0]);

               }
               whereInTheListAmI++;
           }

           if (itemsNotFound.Any())
           {
               Console.WriteLine("Some items couldn\'t be found. Do you still want to copy the rest of the items?");
               string unfoundItemsInput = Console.ReadLine();

               if (unfoundItemsInput == "true")
               {
                   Console.WriteLine("Cancelling copying items.");
                   return;
               }

           }

           var putQuery = new PutItems();

           foreach (Item i in items)
           {
               i.EntityId = destinationEntityId;

           }

           putQuery.Add(items);
           Session.ExecuteAsAdmin(destinationEnvironment, putQuery);
           Console.WriteLine("Copy Items Complete...");
           
            
        }

        private static void CopyResources( string destId, string sourceId, string sourcePath, string destPath, string from, string to)
        {
            var cmd = new GetResourceList()
            {
                EntityId = sourceId,
                ResourcePath = sourcePath
            };
            Session.ExecuteAsAdmin(from, cmd);

            foreach (Bfw.Agilix.DataContracts.Resource res in cmd.Resources)
            {

                string t = "test";
                var cmdGet = new GetResource()
                {
                    EntityId = sourceId,
                    ResourcePath = res.Url
                };

                Session.ExecuteAsAdmin(from, cmdGet);

                var resToMove = cmdGet.Resource;
                resToMove.EntityId = destId;
                var cmdPut = new PutResource()
                {
                    Resource = resToMove
                };
                Session.ExecuteAsAdmin(to, cmdPut);

            }
        }

        private static void StoreItems(string entityId, List<Item> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("No items to process for : " + entityId);
                return;
            }

            var cmd = new PutItems();
            cmd.Add(items);
            Session.Execute(cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static void StoreItems(string entityId, List<Item> items, String environment)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("No items to process for : " + entityId);
                return;
            }

            var cmd = new PutItems();
            cmd.Add(items);
            Session.ExecuteAsAdmin(environment, cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static GetItems ListChildren(string entityId, string parentId)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='" + parentId + "'",
                    Depth = 1
                }
            };
            return query;
        }

        private static GetItems ListChildren(string entityId, string parentId, int depth)
        {
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = entityId,
                    Query = "/parent='" + parentId + "'",
                    Depth = depth
                }
            };
            return query;
        }

        private static GetItems ListChildren(string entityId, string parentId, int depth, string categoryId)
        {
            var itemSearch = ItemQueryHelper.BuildListChildrenQuery(entityId, parentId, depth, categoryId, "");

            var query = new GetItems()
            {
                SearchParameters = itemSearch
            };
            return query;
        }


        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static OptionSet RegisterOptions(CommandOptions options)
        {
            var os = new OptionSet();
            os.Add("a:", a => options.SourceEntityId = a);
            os.Add("b:", b => options.DestinationEntityId = b);

            os.Add("c:", c => options.SourceItemId = c);
            os.Add("d:", d => options.DestinationItemId = d);

            os.Add("e:", e => options.SourceEnvironment = e);
            os.Add("f:", f => options.DestinationEnvironment = f);

            os.Add("g:", e => options.SourcePath = e);
            os.Add("h:", f => options.DestinationPath = f);

            os.Add("i:", i => options.Query = i);

            os.Add("rec:", rec => options.Recursive = rec);

            os.Add("action:", action => options.Action = action);

            os.Add("filename:", i => options.FileName = i);

            return os;
        }

        private static ISessionManager EstablishConnection()
        {
            var username = ConfigurationManager.AppSettings["user"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, null);
            return sm;
        }

    }

    public class CommandOptions
    {
        /// <summary>
        /// Metadata key to add or overwrite
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Value of the Metadata key to use
        /// </summary>
        public string Value { get; set; }
        /// <summary>
        /// Id of the folder whose items should be marked
        /// </summary>
        public string FolderId { get; set; }

        /// <summary>
        /// Id of the course the folder exists in
        /// </summary>
        public string CourseId { get; set; }

        public PropertyType Type { get; set; }

        public string Action { get; set; }

        /// <summary>
        /// Id of the course to copy items to
        /// </summary>
        public string ProductCourseId { get; set; }

        /// <summary>
        /// Entity Id the item is coming from
        /// </summary>
        public string DestinationEntityId { get; set; }

        /// <summary>
        /// Entity Id the item is going to
        /// </summary>
        public string SourceEntityId { get; set; }

        /// <summary>
        /// Item Id the item is coming to
        /// </summary>
        public string DestinationItemId { get; set; }

        /// <summary>
        /// Item Id the item is going to
        /// </summary>
        public string SourceItemId { get; set; }

        /// <summary>
        /// Environment item is Coming from
        /// </summary>
        public string DestinationEnvironment { get; set; }

        /// <summary>
        /// Environment From where the item is Going to
        /// </summary>
        public string SourceEnvironment { get; set; }

        /// <summary>
        /// Destination the Resource is coming from
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Destination the Resource is going to
        /// </summary>
        public string DestinationPath { get; set; }

        public string Query {get; set;}

        public string Recursive { get; set; }

        public string FileName { get; set; }
    
    }
}
