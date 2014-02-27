using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.Dlap.Session
{
    /// <summary>
    /// Implementors are able to transform their internal state into a DlapRequest
    /// </summary>
    public interface IDlapRequestTransformer
    {
        /// <summary>
        /// Transforms object state into a valid DlapRequest
        /// </summary>
        /// <returns>DlapRequest formed from object state</returns>
        DlapRequest ToRequest();
    }
}
