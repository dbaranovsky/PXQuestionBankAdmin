using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PXWebAPI.Models
{
    /// <summary>
    /// The following XML should be used to store instructor console settings
    /// <bfw_console_settings> 
    ///   <show_general>true</show_general> 
    ///   <show_navigation>false</show_navigation> 
    ///   <show_launchpad>true</show_launchpad> 
    ///</bfw_console_settings> 
    /// </summary>
    public class InstructorConsoleSettings : Widget
    {
        public InstructorConsoleSettings()
        {
            ShowGeneral = true;
            ShowNavigation = true;
            ShowLaunchPad = true;
        }

        public bool ShowGeneral { get; set; }

        public bool ShowNavigation { get; set; }

        public bool ShowLaunchPad { get; set; }

        public bool ShowChapters { get; set; }

        public bool ShowTypes { get; set; }

        public bool ShowEbook { get; set; }

        public bool ShowMyResources { get; set; }

        public Dictionary<string, string> FacetsToShow { get; set; }

        public List<SettingResource> Resources { get; set; }

        public LaunchPadSettings LaunchPadSettings = new LaunchPadSettings();

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
