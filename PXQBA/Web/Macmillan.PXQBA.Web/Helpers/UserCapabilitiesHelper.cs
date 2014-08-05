using System.Collections.Generic;
using System.Web;
using Macmillan.PXQBA.Business.Models;

namespace Macmillan.PXQBA.Web.Helpers
{
    public class UserCapabilitiesHelper
    {
        private const string CapabilitiesParamName = "user_capabilities";

        public static IEnumerable<Capability> Capabilities
        {
            get { return HttpContext.Current.Session[CapabilitiesParamName] as IEnumerable<Capability>; }
            set { HttpContext.Current.Session[CapabilitiesParamName] = value; }
        }

        public static IEnumerable<Capability> GetCapabilities(string courseId)
        {
            return HttpContext.Current.Session[CapabilitiesParamName] as IEnumerable<Capability>;
        }
    }
}