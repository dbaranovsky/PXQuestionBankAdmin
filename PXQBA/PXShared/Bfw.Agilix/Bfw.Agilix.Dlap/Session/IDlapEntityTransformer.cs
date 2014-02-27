using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors can transform their internal state to a DLAP entity to XML
    /// </summary>
    public interface IDlapEntityTransformer
    {
        /// <summary>
        /// Transforms internal object state into an XElement representation of a DLAP entity
        /// </summary>
        /// <returns>XElement containing the transformed object state</returns>
        XElement ToEntity();
    }
}
