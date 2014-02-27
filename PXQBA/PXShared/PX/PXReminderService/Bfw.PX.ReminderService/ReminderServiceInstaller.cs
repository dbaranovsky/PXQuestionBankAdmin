using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration.Install;
using System.ComponentModel;
using System.ServiceProcess;

namespace Bfw.PX.ReminderService
{
    [RunInstaller(true)]
    public class ReminderServiceInstaller : Installer
    {
        /// <summary>
        /// public constructor
        /// </summary>
        public ReminderServiceInstaller()
        {
            ConfigureService();
        }

        private ServiceProcessInstaller processInstaller;
        private ServiceInstaller serviceInstaller;

        /// <summary>
        /// Sets the configuration details of the windows service
        /// </summary>
        private void ConfigureService()
        {
            this.processInstaller = new ServiceProcessInstaller();

            this.serviceInstaller = new ServiceInstaller();
            this.serviceInstaller.ServiceName = "PX Reminder Service";
            this.serviceInstaller.Description = "Email reminder service for PX assignments";

            this.processInstaller.Account = ServiceAccount.LocalSystem;

            this.Installers.Add(serviceInstaller);
            this.Installers.Add(processInstaller);
        }
    }
}
