using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bfw.PX.Biz.DataContracts
{
    public class Signal
    {
        public string SignalId { get; set; }
        public string DomainId { get; set; }
        public string Type { get; set; }
        public string EntityId { get; set; }
        public DateTime CreationDate { get; set; }
        public string CreationBy { get; set; }
    }
}
