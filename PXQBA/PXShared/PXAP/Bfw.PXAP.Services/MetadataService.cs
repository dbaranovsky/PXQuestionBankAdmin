using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.PXAP.Components;
using Bfw.PXAP.ServiceContracts;
using Bfw.PXAP.Models;
using Bfw.Agilix.DataContracts;
using Bfw.Agilix.Commands;
using Bfw.Agilix.Dlap;
using Bfw.Agilix.Dlap.Session;
using Bfw.Common.Patterns.Unity;
using Bfw.Common.Collections;
using System.Configuration;


namespace Bfw.PXAP.Services
{
    public class MetadataService : IMetadataService
    {

        private ISession Session { get; set; }
        private IApplicationContext Context { get; set; }

        public MetadataService(IApplicationContext context)
        {
            Context = context;
        }

        public void AddMetadata(string entityId, string parentId, string xmlField, bool bExact, string sValue, Int64 processId, string parentCategory, bool recursive = true)
        {
            ConfigureServiceLocator();
            var sm = EstablishConnection();
            Session = sm.CurrentSession;

            List<Item> items = new List<Item>();
            int percentageDone = 0;

            IProgressService progress = new ProgressService();
            String message = "";
            ProgressModel progModel = new ProgressModel() { ProcessId = processId, Percentage = 0, Status = "Processing" };

            ItemsToCategorize(entityId, parentId, xmlField, bExact, sValue, items, parentCategory, recursive);
            if (items.Count > 20)
            {

                int skip = 0;

                //now set up the progress component
                
                progress.AddUpdateProcess(progModel, out message);

                List<Item> records = new List<Item>();
                do
                {
                    //putting group: " + skip
                    records = items.Skip(skip).Take(20).ToList();

                    if (records.Count > 0)
                    {
                        percentageDone = Convert.ToInt32(skip * 100 / items.Count);
                        progModel.Percentage = percentageDone;
                        progress.AddUpdateProcess(progModel, out message);
                        StoreItems(entityId.ToString(), records);
                    }
                    skip += 20;

                } while (records.Count >= 20);

            }
            else
                StoreItems(entityId.ToString(), items);

            //update the progress to 100%
            progModel.Percentage = 100;
            progress.AddUpdateProcess(progModel, out message);

        }

        private void ItemsToCategorize(string entityId, string parentId, string xmlField, bool bExact, string sValue, List<Item> items, string parentCategory, bool recursive = true)
        {
            
            var cmd = ListChildren(entityId.ToString(), parentId.ToString(), parentCategory);
            Session.ExecuteAsAdmin(Context.Environment, cmd);
            foreach (var item in cmd.Items)
            {
                bool hasProperty = item.Data.Elements(xmlField).Any();
                if (!hasProperty)
                {
                    item.Data.Add(new XElement(xmlField, sValue));
                }
                else
                {
                    item.Data.Element(xmlField).Value = sValue;
                }

                //now to deal with the bExact.  if it is true, need to tack an attribute ‘dlaptype=”exact" to the element
                bool hasAttribute = item.Data.Elements(xmlField).Attributes("dlaptype").Any();
                if (hasAttribute && !bExact) //need to remove it
                {
                    item.Data.Elements(xmlField).Attributes("dlaptype").Remove();
                } //no else as there is no change if it already has the attribute

                if (!hasAttribute && bExact)
                {
                    item.Data.Element(xmlField).Add(new XAttribute("dlaptype", "exact"));
                }
                items.Add(item);
                if (recursive)
                {
                    ItemsToCategorize(entityId, item.Id, xmlField, bExact, sValue, items, parentCategory);
                }

            }
        }

        private GetItems ListChildren(string entityId, string parentId, string parentCategory)
        {
            string queryFilter = "";
            if (string.IsNullOrEmpty(parentCategory))
            {
                queryFilter = "/parent='" + parentId + "'";
            }
            else
            {
                queryFilter = string.Format(@"/bfw_tocs/{0}@parentId='{1}'", parentCategory, parentId);
                //deprecated legacy faceplate parent category 
                if (parentCategory == "syllabusfilter")
                {
                    queryFilter += " OR " + string.Format(@"/bfw_tocs/{0}='{1}'", parentId, parentId);
                }
                //end legacy deprecated
            }
            var query = new GetItems()
            {
                SearchParameters = new ItemSearch()
                {
                    EntityId = entityId,
                    Query = queryFilter,
                    Depth = 1
                }
            };
            return query;
        }

        private void StoreItems(string entityId, List<Item> items)
        {
            if (items.Count == 0)
            {
                Console.WriteLine("No items to process for : " + entityId);
                return;
            }

            var cmd = new PutItems();
            cmd.Add(items);
            Session.ExecuteAsAdmin(Context.Environment, cmd);
            if (!cmd.Failures.IsNullOrEmpty())
            {
                foreach (var failure in cmd.Failures)
                {
                    Console.WriteLine("Failed putting item {0} with reason {1}", failure.ItemId, failure.Reason);
                }
            }
        }

        private static void ConfigureServiceLocator()
        {
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();
            ServiceLocator.SetLocatorProvider(() => locator);
        }

        private static ISessionManager EstablishConnection()
        {
            
            var username = ConfigurationManager.AppSettings["user"];
            var userid = ConfigurationManager.AppSettings["userid"];
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, true, string.Empty);
            return sm;
        }
    }


}
