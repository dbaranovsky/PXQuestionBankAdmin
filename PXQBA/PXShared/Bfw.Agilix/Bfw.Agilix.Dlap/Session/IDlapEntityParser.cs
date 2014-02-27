using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors can parse XML representation of a specific DLAP entity, e.g. Course, Item, etc
    /// </summary>
    public interface IDlapEntityParser
    {
        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        void ParseEntity(XElement element);
    }
}
