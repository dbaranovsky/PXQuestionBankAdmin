using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.PX.Biz.ServiceContracts
{
    /// <summary>
    /// Administrative actions against DLAP.
    /// </summary>
    public interface IAdminActions
    {
        /// <summary>
        /// Gets the status message from the DLAP server.
        /// </summary>
        /// <returns>DLAP Status message</returns>
        XDocument GetStatus();
    }
}
