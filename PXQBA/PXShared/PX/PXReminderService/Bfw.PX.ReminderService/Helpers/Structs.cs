using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.ReminderService.Helpers
{
    public static class Structs
    {
        public struct EmailTemplate
        { 
            public int id;
            public string TemplateText;
            public string TemplateHtml;
        }
    }
}
