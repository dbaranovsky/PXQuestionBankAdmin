using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.PXPub.Models
{
    public class InstructorConsoleSettings
    {
        public bool ShowGeneral { get; set; }

        public bool ShowNavigation { get; set; }

        public bool ShowLaunchPad { get; set; }

        public bool ShowWelcomeReturn { get; set; }

        public bool ShowBatchUpdater { get; set; }

        public bool ShowManageAnnouncemets { get; set; }

        public bool ShowChapters { get; set; }

        public bool ShowTypes { get; set; }

        public bool ShowEbook { get; set; }

        public bool ShowMyResources { get; set; }

        public Dictionary <string, string> FacetsToShow { get; set; }
        
        public List<SettingResource> Resources { get; set; }

        public LaunchPadSettings LaunchPadSettings = new LaunchPadSettings();

        public InstructorConsoleSettings()
        {
            Resources = new List<SettingResource>();
        }

        public class SettingResource
        {
            public String Name { get; set; }
            public String Type { get; set; }
            public String Value { get; set; }

            public SettingResource(string name, string value, string type)
            {
                this.Name = name;
                this.Type = type;
                this.Value = value;
            }

        }
    }
}
