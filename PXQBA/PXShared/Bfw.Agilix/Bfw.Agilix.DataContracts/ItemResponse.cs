using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bfw.Agilix.Dlap.Session;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Notes entered about an item.
    /// </summary>
    public abstract class ItemResponse : IDlapEntityParser
    {
        /// <summary>
        /// Notes entered about an item.
        /// </summary>
        public string Notes { get; set; }

        #region IDlapEntityParser Members

        /// <summary>
        /// Parses an XElement into internal object state. This allows for objects to be decomposed from
        /// parts of Dlap responses.
        /// </summary>
        /// <param name="element">element that contains the state to parse</param>
        /// <remarks></remarks>
        public virtual void ParseEntity(System.Xml.Linq.XElement element)
        {
            var notes = element.Element(ElStrings.Notes);
            if (notes != null)
            {
                Notes = notes.Value;
            }
        }

        #endregion
    }
}
