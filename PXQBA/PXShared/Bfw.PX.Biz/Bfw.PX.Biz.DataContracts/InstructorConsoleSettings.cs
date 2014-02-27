using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using BizDC = Bfw.PX.Biz.DataContracts;

namespace Bfw.PX.Biz.DataContracts
{
[Serializable]
    /// <summary>
    /// The following XML should be used to store instructor console settings
    /// <bfw_console_settings> 
    ///   <show_general>true</show_general> 
    ///   <show_navigation>false</show_navigation> 
    ///   <show_launchpad>true</show_launchpad> 
    ///   <show_welcome_return>true</show_welcome_return> 
    ///</bfw_console_settings> 
    /// </summary>
    public class InstructorConsoleSettings
    {
        public InstructorConsoleSettings()
        {
            ShowGeneral = true;
            ShowNavigation = true;
            ShowLaunchPad = true;
            ShowWelcomeReturn = true;
            ShowManageAnnouncemets = true;
        }
        public void ParseSettings(XElement elem)
        {
            ShowNavigation = false;
            ShowGeneral = false;
            ShowLaunchPad = false;
            ShowWelcomeReturn = false;
            ShowBatchUpdater = false;
            ShowManageAnnouncemets = false;

            if (elem != null)
            {
                Resources = new List<SettingResource>();
                foreach (XElement res in elem.Elements())
                {
                    // a resource object has a name, type, value
                    // based on the a resource tag, we create our settings object
                    var name = res.Attribute("name") == null ? "" : res.Attribute("name").Value;
                    var type = res.Attribute("type") == null ? "" : res.Attribute("type").Value;
                    var value = res.Attribute("value") == null ? "" : res.Attribute("value").Value;

                    switch (type)
                    {
                            // following properties are the more generic settings and are included in the settings model
                        case "showchapters":
                            ShowChapters = true;
                            break;
                        case "showtypes":
                            ShowTypes = true;
                            break;
                        case "showebook":
                            ShowEbook = true;
                            break;
                        case "showmyresources":
                            ShowMyResources = true;
                            break;
                        case "showgeneral":
                            ShowGeneral = true;
                            break;
                        case "shownavigation":
                            ShowNavigation = true;
                            break;
                        case "showlaunchpad":
                            ShowLaunchPad = true;
                            break;
                        case "showwelcomereturn":
                            ShowWelcomeReturn = true;
                            break;
                        case "showbatchupdater":
                            ShowBatchUpdater = true;
                            break;
                        case "showmanageannouncemets":
                            ShowManageAnnouncemets = true;
                            break;
                        default:
                            Resources.Add(new SettingResource(name, value, type));
                            break;
                    }
                }
            }
        }

        public bool ShowGeneral { get; set; }

        public bool ShowNavigation { get; set; }

        public bool ShowLaunchPad { get; set; }

        public bool ShowWelcomeReturn { get; set; }

        public bool ShowBatchUpdater { get; set; }

        public bool ShowManageAnnouncemets { get; set; }

        public bool ShowChapters { get; set; }

        public bool ShowTypes{ get; set; }

        public bool ShowEbook { get; set; }

        public bool ShowMyResources { get; set; }

        public Dictionary<string, string> FacetsToShow { get; set; }

        public List<SettingResource> Resources { get; set; }

        public LaunchPadSettings LaunchPadSettings = new LaunchPadSettings();

        [Serializable]
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
