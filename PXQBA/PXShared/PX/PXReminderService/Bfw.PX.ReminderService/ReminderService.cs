using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using Bfw.Common.Patterns.Unity;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Unity;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Bfw.PX.ReminderService
{
    class ReminderService : ServiceBase
    {
        /// <summary>
        /// Service timer
        /// </summary>
        Timer serviceTimer;

        /// <summary>
        /// Reminder Action
        /// </summary>
        public ReminderAction Reminder 
        { 
            get 
            {
                var reminder = ReminderAction.GetInstance();
                (ServiceLocator.Current as UnityServiceLocator).Container.BuildUp(typeof(ReminderAction), reminder, "ReminderAction");

                return reminder;
            } 
        }

        /// <summary>
        /// Creating a private object of EventLog to be used
        /// </summary>
        private static EventLog eventLog = new EventLog(logName: "PxReminder", machineName: ".", source: "PxReminderService");

        /// <summary>
        /// public constructor
        /// </summary>
        public ReminderService()
        {
            try
            {
                this.ServiceName = "PX Reminder Service";

                ConfigureUnity();

                SetEventLogSource();
                serviceTimer = new Timer(50);
                serviceTimer.Elapsed += new ElapsedEventHandler(TimerElapsed);
                
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderService: Error initializing service {0}", ex.StackTrace), EventLogEntryType.Error);
            }
        }

        private void SetEventLogSource()
        {
            if (!EventLog.SourceExists("PxReminderService"))
            {
                EventLog.CreateEventSource("PxReminderService", "PxReminder");
            }
        }

        /// <summary>
        /// Timer elapsed event for the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            serviceTimer.Stop();
            
            try
            {
                Reminder.Run();
                serviceTimer.Interval = Reminder.ReminderInterval;
            }
            catch (Exception ex)
            {
                eventLog.WriteEntry(string.Format("ReminderService: Error sending reminders {0}", ex.StackTrace), EventLogEntryType.Error);
            }
            finally
            {
                serviceTimer.Start();
            }
        }

        /// <summary>
        /// Disposes the objects
        /// </summary>
        /// <param name="disposing">Whether or not disposing is going on.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (serviceTimer != null))
            {
                serviceTimer.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Override of the service startup method
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            var interval = serviceTimer.Interval;
            eventLog.WriteEntry(string.Format("Reminder service started with interval {0}", interval), EventLogEntryType.Information);
            serviceTimer.Start();

            base.OnStart(args);
        }

        /// <summary>
        /// Override of the service stop method
        /// </summary>
        protected override void OnStop()
        {
            eventLog.WriteEntry("Reminder service stopped", EventLogEntryType.Information);
            serviceTimer.Stop();

            base.OnStop();
        }

        /// <summary>
        /// Main thread.
        /// </summary>
        public static void Main()
        {
            ServiceBase.Run(new ReminderService());
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
