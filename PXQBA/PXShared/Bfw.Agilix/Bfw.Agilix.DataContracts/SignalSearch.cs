using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.Agilix.DataContracts
{
    /// <summary>
    /// Parameters for searching signals.
    /// </summary>
    public class SignalSearch
    {
        /// <summary>
        /// Id of the last signal to include.
        /// </summary>
        public string LastSignalId { get; set; }

        /// <summary>
        /// Type of signal to find.
        /// </summary>
        public string SignalType { get; set; }
    }
}
