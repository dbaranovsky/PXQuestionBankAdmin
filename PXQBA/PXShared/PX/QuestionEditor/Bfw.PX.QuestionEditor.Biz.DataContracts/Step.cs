using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Bfw.PX.QuestionEditor.Biz.DataContracts
{
    [DataContract]
    public class Step
    {
        [DataMember]
        public List<Property> ShortAnswer { get; set; }

        [DataMember]
        public List<Property> MultiChoice { get; set; }

        [DataMember]
        public string Question { get; set; }

        [DataMember]
        public string Hint { get; set; }

        [DataMember]
        public string Correct { get; set; }

        [DataMember]
        public string Incorrect { get; set; }        

        [DataMember]
        public int Sequence { get; set; }

        [DataMember]
        public string Id { get; set; }

        [DataMember]
        public int Points { get; set; }
    }
}
