using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Constraint
    {
        [DataMember]
        public string Exclusions { get; set; }

        [DataMember]
        public string Inclusions { get; set; }

        [DataMember]
        public Condition Condition { get; set; }

        [DataMember]
        public List<Range> Ranges { get; set; }

        public Constraint()
        {            
        }
    }
}
