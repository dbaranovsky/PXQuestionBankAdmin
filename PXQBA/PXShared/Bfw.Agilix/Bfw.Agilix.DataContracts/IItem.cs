using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Implementors are able to turn their internal state into an Item object.
    /// </summary>
    public interface IItem
    {
        /// <summary>
        /// Implementor can turn its internal state into an Agilix Item
        /// </summary>
        /// <returns></returns>
        Item AsItem();
    }
}
