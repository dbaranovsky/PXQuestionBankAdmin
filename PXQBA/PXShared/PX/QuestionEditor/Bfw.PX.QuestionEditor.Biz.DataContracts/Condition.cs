using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Condition
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Expression1 { get; set; }

        [DataMember]
        public string Expression2 { get; set; }
    }
}
