using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Timers;

namespace ReportsWS
{
    public partial class ReportService : ServiceBase
    {
        private Timer _timer = null;

        public ReportService()
        {
            InitializeComponent();
            // Set the timer to fire every sixty seconds

            // (remember the timer is in millisecond resolution, 

            //  so 1000 = 1 second. ) 

            _timer = new Timer(600000);

        }

        protected override void OnStart(string[] args)
        {
            _timer.Start();
            LoadData data = new LoadData();
            LoadData.LoadMethods(); 
        }

        protected override void OnStop()
        {
            _timer.Stop();

        }
    }
}
