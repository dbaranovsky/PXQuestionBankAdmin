using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Common.Patterns.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.ReminderService
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigureUnity();

            var singleton = ReminderAction.GetInstance();
            (ServiceLocator.Current as UnityServiceLocator).Container.BuildUp(typeof(ReminderAction), singleton, "ReminderAction");

            singleton.Run();

            //var svc = new RAService();
            //var courses = svc.GetCourseList(new List<string>() { "53436", "107784", "87265" });

            var y = 0;
        }

        /// <summary>
        /// Configure the unity service locator
        /// </summary>
        private static void ConfigureUnity()
        {
            //Set the unity service locator
            var locator = new UnityServiceLocator();
            locator.Container.AddNewExtensionIfNotPresent<EnterpriseLibraryCoreExtension>();
            locator.Container.AddNewExtensionIfNotPresent<LoggingBlockExtension>();

            ServiceLocator.SetLocatorProvider(() => locator);
        }

    }
}
