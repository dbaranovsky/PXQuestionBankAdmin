using System;
using System.IO;
using System.Configuration;

using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;

using Bfw.Common;
using Bfw.Common.Web;
using Bfw.Common.Patterns.Unity;

using Bfw.Agilix.Services;
using Bfw.Agilix.Dlap.Session;
using Bfw.Agilix.DataContracts;

namespace PutRes
{
    class Program
    {
        static void Main(string[] args)
        {
            // Big try catch to get some more reasonable errors from the application
            try
            {
                string entityId = null,
                    filePath = null,
                    resourcePath = null;

                // Get the arguments and make sure they are all valid
                try
                {
                    entityId = args[0];
                    filePath = args[1];
                    resourcePath = args[2];
                }
                catch
                {
                    ShowUsageAndExit(1);
                }

                if (resourcePath.StartsWith("/"))
                {
                    resourcePath = resourcePath.Substring(1);
                }

                ConfigureServiceLocator();
                ISessionManager sm = EstablishConnection();

                ItemService itemService = new ItemService(sm);

                Resource r = new Resource()
                {
                    ContentType = ContentTypeUtils.GetContentType(filePath),
                    Url = "Assets/" + resourcePath,
                    EntityId = entityId,
                    Extension = new FileInfo(filePath).Extension
                };

                FileStream fileStream = null;
                try
                {
                    fileStream = File.Open(filePath, FileMode.Open);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ShowUsageAndExit(1);
                }
                Stream resStream = r.GetStream();
                StreamExtensions.Copy(fileStream, resStream);
                resStream.Flush();

                itemService.StoreResource(r);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                ShowUsageAndExit(1);
            }

            Exit(0);
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
            var password = ConfigurationManager.AppSettings["password"];
            var sm = ServiceLocator.Current.GetInstance<ISessionManager>();
            sm.CurrentSession = sm.StartNewSession(username, password, "");

            return sm;
        }

        static void ShowUsageAndExit(int code)
        {
            Console.WriteLine("Usage: putres entityid filename resourcename");
            Exit(code);
        }

        static void Exit(int code)
        {
            Environment.Exit(code);
        }
    }
}
